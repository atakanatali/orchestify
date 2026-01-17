'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useParams } from 'next/navigation';
import { boardsApi, tasksApi, Task } from '@/lib/api';
import { Plus, Loader2, Play, MoreHorizontal, Clock, CheckCircle2, Circle, AlertCircle } from 'lucide-react';
import clsx from 'clsx';
import { useState } from 'react';

const COLUMNS = [
    { id: 'Todo', title: 'To Do', color: 'bg-gray-400' },
    { id: 'InProgress', title: 'In Progress', color: 'bg-[var(--warning)]' },
    { id: 'Done', title: 'Done', color: 'bg-[var(--success)]' },
];

export default function BoardPage() {
    const { workspaceId, boardId } = useParams<{ workspaceId: string; boardId: string }>();
    const queryClient = useQueryClient();
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);

    const { data: board } = useQuery({
        queryKey: ['board', boardId],
        queryFn: () => boardsApi.get(workspaceId, boardId),
        enabled: !!boardId,
    });

    const { data: tasks, isLoading } = useQuery({
        queryKey: ['tasks', boardId],
        queryFn: () => tasksApi.list(boardId),
        enabled: !!boardId,
    });

    const runMutation = useMutation({
        mutationFn: (taskId: string) => tasksApi.run(boardId, taskId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['tasks', boardId] });
        },
    });

    const moveMutation = useMutation({
        mutationFn: ({ taskId, status }: { taskId: string; status: string }) =>
            tasksApi.move(boardId, taskId, { status }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['tasks', boardId] });
        },
    });

    if (isLoading) {
        return (
            <div className="flex items-center justify-center h-full">
                <Loader2 className="animate-spin text-[var(--primary)]" size={32} />
            </div>
        );
    }

    const tasksByStatus = COLUMNS.reduce((acc, col) => {
        acc[col.id] = tasks?.items?.filter(t => t.status === col.id) || [];
        return acc;
    }, {} as Record<string, Task[]>);

    return (
        <div className="h-full flex flex-col">
            {/* Header */}
            <div className="p-6 border-b border-[var(--border)] bg-white">
                <div className="flex items-center justify-between">
                    <div>
                        <h1 className="text-2xl font-bold text-[var(--text-primary)]">
                            {board?.board?.name}
                        </h1>
                        <p className="text-[var(--text-secondary)]">
                            {board?.board?.description || 'No description'}
                        </p>
                    </div>
                    <button className="flex items-center gap-2 px-4 py-2.5 bg-[var(--primary)] text-white rounded-lg hover:bg-[var(--primary-dark)] transition-colors shadow-sm">
                        <Plus size={18} />
                        Add Task
                    </button>
                </div>
            </div>

            {/* Kanban Board */}
            <div className="flex-1 overflow-x-auto p-6">
                <div className="flex gap-5 h-full min-w-max">
                    {COLUMNS.map((column) => (
                        <KanbanColumn
                            key={column.id}
                            column={column}
                            tasks={tasksByStatus[column.id]}
                            onRunTask={(taskId) => runMutation.mutate(taskId)}
                            onMoveTask={(taskId, status) => moveMutation.mutate({ taskId, status })}
                            onSelectTask={setSelectedTask}
                        />
                    ))}
                </div>
            </div>

            {/* Task Detail Modal */}
            {selectedTask && (
                <TaskDetailModal task={selectedTask} onClose={() => setSelectedTask(null)} boardId={boardId} />
            )}
        </div>
    );
}

function KanbanColumn({
    column,
    tasks,
    onRunTask,
    onMoveTask,
    onSelectTask
}: {
    column: { id: string; title: string; color: string };
    tasks: Task[];
    onRunTask: (taskId: string) => void;
    onMoveTask: (taskId: string, status: string) => void;
    onSelectTask: (task: Task) => void;
}) {
    return (
        <div className="w-80 flex-shrink-0 flex flex-col bg-[var(--background)] rounded-xl">
            {/* Column Header */}
            <div className="flex items-center gap-3 p-4">
                <div className={`w-3 h-3 rounded-full ${column.color}`} />
                <h3 className="font-semibold text-[var(--text-primary)]">{column.title}</h3>
                <span className="px-2 py-0.5 bg-white rounded-full text-xs font-medium text-[var(--text-secondary)]">
                    {tasks.length}
                </span>
            </div>

            {/* Tasks */}
            <div className="flex-1 overflow-y-auto p-2 space-y-3">
                {tasks.sort((a, b) => a.orderKey - b.orderKey).map((task) => (
                    <TaskCard
                        key={task.id}
                        task={task}
                        onRun={() => onRunTask(task.id)}
                        onClick={() => onSelectTask(task)}
                    />
                ))}

                {/* Add Task Button */}
                <button className="w-full p-3 border-2 border-dashed border-[var(--border)] rounded-lg text-[var(--text-secondary)] hover:border-[var(--primary)] hover:text-[var(--primary)] hover:bg-white transition-all flex items-center justify-center gap-2">
                    <Plus size={16} />
                    Add Task
                </button>
            </div>
        </div>
    );
}

function TaskCard({ task, onRun, onClick }: { task: Task; onRun: () => void; onClick: () => void }) {
    return (
        <div
            onClick={onClick}
            className="bg-white rounded-xl p-4 border border-[var(--border)] shadow-sm hover:shadow-md hover:border-[var(--primary)] transition-all cursor-pointer group"
        >
            <div className="flex items-start justify-between mb-3">
                <h4 className="font-medium text-[var(--text-primary)] group-hover:text-[var(--primary)] transition-colors line-clamp-2">
                    {task.title}
                </h4>
                <button
                    onClick={(e) => { e.stopPropagation(); }}
                    className="p-1 rounded opacity-0 group-hover:opacity-100 hover:bg-gray-100 transition-all"
                >
                    <MoreHorizontal size={16} className="text-[var(--text-secondary)]" />
                </button>
            </div>

            {task.description && (
                <p className="text-sm text-[var(--text-secondary)] line-clamp-2 mb-3">
                    {task.description}
                </p>
            )}

            <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                    <PriorityBadge priority={task.priority} />
                    {task.attemptCount > 0 && (
                        <span className="text-xs text-[var(--text-secondary)]">
                            {task.attemptCount} runs
                        </span>
                    )}
                </div>

                {task.status !== 'Done' && (
                    <button
                        onClick={(e) => { e.stopPropagation(); onRun(); }}
                        className="flex items-center gap-1 px-2.5 py-1.5 bg-[var(--success)] text-white text-xs font-medium rounded-lg hover:bg-green-600 transition-colors opacity-0 group-hover:opacity-100"
                    >
                        <Play size={12} />
                        Run
                    </button>
                )}
            </div>
        </div>
    );
}

function PriorityBadge({ priority }: { priority: number }) {
    const colors = ['bg-gray-200', 'bg-blue-100 text-blue-700', 'bg-yellow-100 text-yellow-700', 'bg-orange-100 text-orange-700', 'bg-red-100 text-red-700', 'bg-red-200 text-red-800'];
    return (
        <span className={clsx('px-2 py-0.5 rounded text-xs font-medium', colors[priority] || colors[0])}>
            P{priority}
        </span>
    );
}

function TaskDetailModal({ task, onClose, boardId }: { task: Task; onClose: () => void; boardId: string }) {
    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" onClick={onClose}>
            <div
                className="bg-white rounded-2xl w-full max-w-2xl max-h-[80vh] overflow-hidden shadow-2xl"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="p-6 border-b border-[var(--border)]">
                    <h2 className="text-xl font-bold text-[var(--text-primary)]">{task.title}</h2>
                    <p className="text-[var(--text-secondary)] mt-1">{task.description || 'No description'}</p>
                </div>
                <div className="p-6">
                    <div className="grid grid-cols-2 gap-4 mb-6">
                        <div>
                            <label className="text-sm text-[var(--text-secondary)]">Status</label>
                            <div className="font-medium">{task.status}</div>
                        </div>
                        <div>
                            <label className="text-sm text-[var(--text-secondary)]">Priority</label>
                            <div><PriorityBadge priority={task.priority} /></div>
                        </div>
                        <div>
                            <label className="text-sm text-[var(--text-secondary)]">Attempts</label>
                            <div className="font-medium">{task.attemptCount}</div>
                        </div>
                        <div>
                            <label className="text-sm text-[var(--text-secondary)]">Created</label>
                            <div className="font-medium text-sm">{new Date(task.createdAt).toLocaleDateString()}</div>
                        </div>
                    </div>
                    <div className="flex gap-3">
                        <button className="flex-1 py-2.5 bg-[var(--success)] text-white rounded-lg font-medium hover:bg-green-600 transition-colors flex items-center justify-center gap-2">
                            <Play size={18} />
                            Run Task
                        </button>
                        <button onClick={onClose} className="px-6 py-2.5 border border-[var(--border)] rounded-lg font-medium hover:bg-gray-50 transition-colors">
                            Close
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
