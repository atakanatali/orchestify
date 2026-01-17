const API_BASE = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

async function fetchApi<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${endpoint}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  });
  if (!res.ok) throw new Error(`API Error: ${res.status}`);
  return res.json();
}

// Workspaces
export const workspacesApi = {
  list: () => fetchApi<{ items: Workspace[] }>('/workspaces'),
  get: (id: string) => fetchApi<{ workspace: Workspace }>(`/workspaces/${id}`),
  create: (data: CreateWorkspace) => fetchApi<{ workspace: Workspace }>('/workspaces', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  update: (id: string, data: UpdateWorkspace) => fetchApi<{ workspace: Workspace }>(`/workspaces/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  delete: (id: string) => fetchApi(`/workspaces/${id}`, { method: 'DELETE' }),
};

// Boards
export const boardsApi = {
  list: (workspaceId: string) => fetchApi<{ items: Board[] }>(`/workspaces/${workspaceId}/boards`),
  get: (workspaceId: string, id: string) => fetchApi<{ board: Board }>(`/workspaces/${workspaceId}/boards/${id}`),
  create: (workspaceId: string, data: CreateBoard) => fetchApi<{ board: Board }>(`/workspaces/${workspaceId}/boards`, {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  update: (workspaceId: string, id: string, data: UpdateBoard) => fetchApi<{ board: Board }>(`/workspaces/${workspaceId}/boards/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  delete: (workspaceId: string, id: string) => fetchApi(`/workspaces/${workspaceId}/boards/${id}`, { method: 'DELETE' }),
};

// Tasks
export const tasksApi = {
  list: (boardId: string) => fetchApi<{ items: Task[] }>(`/boards/${boardId}/tasks`),
  get: (boardId: string, id: string) => fetchApi<{ task: Task }>(`/boards/${boardId}/tasks/${id}`),
  create: (boardId: string, data: CreateTask) => fetchApi<{ task: Task }>(`/boards/${boardId}/tasks`, {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  update: (boardId: string, id: string, data: UpdateTask) => fetchApi<{ task: Task }>(`/boards/${boardId}/tasks/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  move: (boardId: string, id: string, data: MoveTask) => fetchApi(`/boards/${boardId}/tasks/${id}/move`, {
    method: 'PATCH',
    body: JSON.stringify(data),
  }),
  run: (boardId: string, id: string) => fetchApi<{ attempt: Attempt }>(`/boards/${boardId}/tasks/${id}/run`, {
    method: 'POST',
  }),
  delete: (boardId: string, id: string) => fetchApi(`/boards/${boardId}/tasks/${id}`, { method: 'DELETE' }),
};

// Attempts
export const attemptsApi = {
  list: (taskId: string) => fetchApi<{ items: Attempt[] }>(`/tasks/${taskId}/attempts`),
  get: (taskId: string, id: string) => fetchApi<Attempt>(`/tasks/${taskId}/attempts/${id}`),
  cancel: (taskId: string, id: string) => fetchApi(`/tasks/${taskId}/attempts/${id}/cancel`, { method: 'POST' }),
  steps: (taskId: string, id: string) => fetchApi<{ items: RunStep[] }>(`/tasks/${taskId}/attempts/${id}/steps`),
};

// Dashboard
export const dashboardApi = {
  stats: () => fetchApi<DashboardStats>('/dashboard/stats'),
};

// Discovery
export const discoveryApi = {
  listRepos: () => fetchApi<RepositoryInfo[]>('/discovery/repos'),
  listBranches: (repoName: string) => fetchApi<string[]>(`/discovery/branches?repoName=${repoName}`),
};

// Task Messages (Antigravity Chat)
export const messagesApi = {
  list: (taskId: string) => fetchApi<TaskMessage[]>(`/tasks/${taskId}/messages`),
  send: (taskId: string, content: string) => fetchApi<TaskMessage>(`/tasks/${taskId}/messages`, {
    method: 'POST',
    body: JSON.stringify({ content }),
  }),
};

export interface TaskMessage {
  id: string;
  content: string;
  sender: 'User' | 'Agent';
  suggestedAction: string | null;
  createdAt: string;
}

export interface AgentToolCall {
  type: string;
  command: string;
  description?: string;
}

export interface RepositoryInfo {
  name: string;
  fullPath: string;
}

// Types
export interface Workspace {
  id: string;
  name: string;
  repositoryPath: string;
  defaultBranch: string;
  totalTasks: number;
  runningTasks: number;
  lastActivityAt: string | null;
  progressPercent: number;
  createdAt: string;
}

export interface Board {
  id: string;
  workspaceId: string;
  name: string;
  description: string;
  totalTasks: number;
  completedTasks: number;
  createdAt: string;
}

export interface Task {
  id: string;
  boardId: string;
  title: string;
  description: string;
  status: 'Todo' | 'In Progress' | 'Review' | 'Done' | 'Cancelled';
  priority: number;
  orderKey: number;
  attemptCount: number;
  createdAt: string;
}

export interface Attempt {
  id: string;
  taskId: string;
  state: string;
  attemptNumber: number;
  queuedAt: string;
  startedAt: string | null;
  completedAt: string | null;
  errorMessage: string | null;
}

export interface RunStep {
  id: string;
  attemptId: string;
  stepType: string;
  name: string;
  state: string;
  sequenceNumber: number;
  startedAt: string | null;
  completedAt: string | null;
  durationMs: number | null;
  errorMessage: string | null;
}

export interface DashboardStats {
  totalWorkspaces: number;
  totalBoards: number;
  totalTasks: number;
  pendingTasks: number;
  inProgressTasks: number;
  completedTasks: number;
  queuedAttempts: number;
  runningAttempts: number;
  failedAttempts: number;
}

export interface CreateWorkspace { name: string; repositoryPath: string; defaultBranch?: string; }
export interface UpdateWorkspace { name: string; defaultBranch?: string; }
export interface CreateBoard { name: string; description?: string; }
export interface UpdateBoard { name: string; description?: string; }
export interface CreateTask { title: string; description?: string; priority?: number; }
export interface UpdateTask { title?: string; description?: string; status?: string; priority?: number; }
export interface MoveTask { status?: string; afterTaskId?: string; }
