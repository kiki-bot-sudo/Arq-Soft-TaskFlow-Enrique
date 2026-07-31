const api = "/api/tasks";
const state = { status: "all", tasks: [] };
const $ = id => document.getElementById(id);
const dialog = $("task-dialog");

document.addEventListener("DOMContentLoaded", async () => {
  if (!await authenticate()) return;
  $("new-task").onclick = $("empty-new").onclick = openCreate;
  $("close-dialog").onclick = $("cancel").onclick = () => dialog.close();
  $("task-form").onsubmit = saveTask;
  $("logout").onclick = logout;
  $("search").oninput = debounce(loadTasks, 250);
  $("priority-filter").onchange = $("sort").onchange = loadTasks;
  document.querySelectorAll(".nav-item").forEach(button => button.onclick = () => {
    document.querySelectorAll(".nav-item").forEach(x => x.classList.remove("active"));
    button.classList.add("active"); state.status = button.dataset.status; loadTasks();
  });
  loadAll();
});

async function authenticate() {
  const response = await fetch("/api/auth/me");
  if (response.status === 401) { location.replace("/welcome.html"); return false; }
  if (!response.ok) { showToast("No fue posible comprobar la sesión.", true); return false; }
  const user = await response.json(); $("current-user").textContent = user.displayName;
  return true;
}
async function logout() {
  await fetch("/api/auth/logout", {method:"POST"}); location.replace("/login.html");
}
async function loadAll() { await Promise.all([loadTasks(), loadStats()]); }
async function loadTasks() {
  const params = new URLSearchParams({ sortBy: $("sort").value });
  if ($("search").value.trim()) params.set("search", $("search").value.trim());
  if ($("priority-filter").value) params.set("priority", $("priority-filter").value);
  if (state.status === "pending" || state.status === "overdue") params.set("isCompleted", "false");
  if (state.status === "completed") params.set("isCompleted", "true");
  try {
    const response = await fetch(`${api}?${params}`);
    if (!response.ok) throw new Error(await errorMessage(response));
    let tasks = await response.json();
    if (state.status === "overdue") tasks = tasks.filter(isOverdue);
    state.tasks = tasks; renderTasks();
  } catch (error) { showToast(error.message, true); }
}
async function loadStats() {
  try {
    const stats = await fetchJson(`${api}/statistics`);
    ["total","pending","completed","overdue"].forEach(key => {
      $(key).textContent = stats[key]; $(`nav-${key}`).textContent = stats[key];
    });
  } catch (error) { showToast(error.message, true); }
}
function renderTasks() {
  const labels = { all:["Todas las tareas","Tu lista completa"], pending:["Pendientes","Tareas por completar"], completed:["Completadas","Buen trabajo"], overdue:["Vencidas","Necesitan tu atención"] };
  [$("list-title").textContent,$("list-subtitle").textContent] = labels[state.status];
  $("empty").hidden = state.tasks.length > 0; $("task-list").hidden = state.tasks.length === 0;
  $("task-list").innerHTML = state.tasks.map(task => `<article class="task-card ${task.isCompleted?"done":""}">
    <input class="check" type="checkbox" ${task.isCompleted?"checked":""} aria-label="Marcar ${escapeHtml(task.title)} como ${task.isCompleted?"pendiente":"completada"}" onchange="toggleTask(${task.id},this.checked)">
    <div class="task-main"><h3>${escapeHtml(task.title)}</h3><p>${escapeHtml(task.description)||"Sin descripción"}</p>
      <div class="task-meta"><span class="badge priority-${task.priority}">${priorityLabel(task.priority)}</span>
      ${task.dueTime?`<span class="${isOverdue(task)?"overdue":""}">${isOverdue(task)?"Vencida · ":""}${formatDate(task.dueTime)}</span>`:"<span>Sin fecha límite</span>"}</div></div>
    <div class="actions"><button class="icon-button" onclick="openEdit(${task.id})" aria-label="Editar">✎</button><button class="icon-button delete" onclick="deleteTask(${task.id})" aria-label="Eliminar">⌫</button></div>
    <div class="subtasks"><div class="subtask-list">${(task.subTasks||[]).map(sub=>`<div class="subtask ${sub.isCompleted?"done":""}"><input class="check" type="checkbox" ${sub.isCompleted?"checked":""} onchange="toggleSubTask(${task.id},${sub.id},this.checked)"><span>${escapeHtml(sub.title)}</span><button onclick="deleteSubTask(${task.id},${sub.id})" aria-label="Eliminar subtarea">×</button></div>`).join("")}</div>
    <form class="subtask-form" onsubmit="addSubTask(event,${task.id})"><input maxlength="100" required placeholder="Agregar subtarea..."><button>＋ Agregar</button></form></div>
  </article>`).join("");
}
function openCreate() {
  $("task-form").reset(); $("task-id").value=""; $("priority").value="Medium";
  $("dialog-title").textContent="Nueva tarea"; $("form-error").textContent=""; dialog.showModal(); $("title").focus();
}
window.openEdit = id => {
  const task = state.tasks.find(x => x.id === id); if (!task) return;
  $("task-id").value=task.id; $("title").value=task.title; $("description").value=task.description;
  $("priority").value=task.priority; $("due-time").value=task.dueTime?task.dueTime.slice(0,16):"";
  $("dialog-title").textContent="Editar tarea"; $("form-error").textContent=""; dialog.showModal();
};
async function saveTask(event) {
  event.preventDefault();
  const id=$("task-id").value, current=state.tasks.find(x=>x.id===Number(id));
  const payload={title:$("title").value.trim(),description:$("description").value.trim(),priority:$("priority").value,dueTime:$("due-time").value?new Date($("due-time").value).toISOString():null};
  if(id) payload.isCompleted=current?.isCompleted??false;
  try {
    const response=await fetch(id?`${api}/${id}`:api,{method:id?"PUT":"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify(payload)});
    if(!response.ok) throw new Error(await errorMessage(response));
    dialog.close(); showToast(id?"Tarea actualizada":"Tarea creada"); await loadAll();
  } catch(error) { $("form-error").textContent=error.message; }
}
window.toggleTask = async (id,isCompleted) => {
  try { await fetchJson(`${api}/${id}/completion`,{method:"PATCH",headers:{"Content-Type":"application/json"},body:JSON.stringify({isCompleted})}); showToast(isCompleted?"Tarea completada":"Tarea marcada como pendiente"); await loadAll(); }
  catch(error){showToast(error.message,true);await loadTasks();}
};
window.deleteTask = async id => {
  const task=state.tasks.find(x=>x.id===id);
  if(!confirm(`¿Eliminar "${task?.title??"esta tarea"}"? Esta acción no se puede deshacer.`)) return;
  try { const response=await fetch(`${api}/${id}`,{method:"DELETE"});if(!response.ok)throw new Error(await errorMessage(response));showToast("Tarea eliminada");await loadAll(); } catch(error){showToast(error.message,true);}
};
window.addSubTask = async (event,taskId) => {
  event.preventDefault(); const input=event.currentTarget.querySelector("input");
  try { await fetchJson(`${api}/${taskId}/subtasks`,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify({title:input.value.trim()})});showToast("Subtarea agregada");await loadTasks(); } catch(error){showToast(error.message,true);}
};
window.toggleSubTask = async (taskId,subTaskId,isCompleted) => {
  try { await fetchJson(`${api}/${taskId}/subtasks/${subTaskId}`,{method:"PATCH",headers:{"Content-Type":"application/json"},body:JSON.stringify({isCompleted})});await loadTasks(); } catch(error){showToast(error.message,true);}
};
window.deleteSubTask = async (taskId,subTaskId) => {
  try { const response=await fetch(`${api}/${taskId}/subtasks/${subTaskId}`,{method:"DELETE"});if(!response.ok)throw new Error(await errorMessage(response));showToast("Subtarea eliminada");await loadTasks(); } catch(error){showToast(error.message,true);}
};
async function fetchJson(url,options){const response=await fetch(url,options);if(!response.ok)throw new Error(await errorMessage(response));return response.json();}
async function errorMessage(response){try{const data=await response.json();return data.message||data.title||Object.values(data.errors||{}).flat()[0]||"No fue posible completar la operación.";}catch{return"Error de comunicación con el servidor.";}}
function isOverdue(task){return !task.isCompleted&&task.dueTime&&new Date(task.dueTime)<new Date();}
function formatDate(value){return new Intl.DateTimeFormat("es-MX",{day:"numeric",month:"short",hour:"2-digit",minute:"2-digit"}).format(new Date(value));}
function priorityLabel(value){return({High:"Alta",Medium:"Media",Low:"Baja"})[value]||value;}
function escapeHtml(value){const div=document.createElement("div");div.textContent=value||"";return div.innerHTML;}
function showToast(message,isError=false){const toast=$("toast");toast.textContent=message;toast.style.background=isError?"#b9414b":"";toast.classList.add("show");setTimeout(()=>toast.classList.remove("show"),2600);}
function debounce(fn,wait){let timer;return()=>{clearTimeout(timer);timer=setTimeout(fn,wait);};}
