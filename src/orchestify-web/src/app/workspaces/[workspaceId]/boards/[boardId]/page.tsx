'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useParams } from 'next/navigation';
import { boardsApi, tasksApi, Task } from '@/lib/api';
import { Plus, Loader2, Play, MoreHorizontal, Trash2, X } from 'lucide-react';
import clsx from 'clsx';
import { useState } from 'react';
import { CreateTaskModal } from '@/components/modals/CreateTaskModal';
import { LiveLogs } from '@/components/workshops/LiveLogs';

const COLUMNS = [
    { id: 'Todo', title: 'To Do', color: 'bg-gray-400' },
    { id: 'InProgress', title: 'In Progress', color: 'bg-[var(--warning)]' },
    { id: 'Done', title: 'Done', color: 'bg-[var(--success)]' },
];

export default function BoardPage() {
    const { workspaceId, boardId } = useParams<{ workspaceId: string; boardId: string }>();
    const queryClient = useQueryClient();
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [showCreateTask, setShowCreateTask] = useState(false);
    const [activeAttempt, setActiveAttempt] = useState<{ id: string; title: string } | null>(null);

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
        onSuccess: (data, taskId) => {
            const task = tasks?.items?.find(t => t.id === taskId);
            if (data.attempt) {
                setActiveAttempt({
                    id: data.attempt.id,
                    title: task?.title || 'Unknown Task'
                });
            }
            queryClient.invalidateQueries({ queryKey: ['tasks', boardId] });
        },
    });


    const deleteMutation = useMutation({
        mutationFn: (taskId: string) => tasksApi.delete(boardId, taskId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['tasks', boardId] });
            setSelectedTask(null);
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
                    <button
                        onClick={() => setShowCreateTask(true)}
                        className="flex items-center gap-2 px-4 py-2.5 bg-[var(--primary)] text-white rounded-lg hover:bg-[var(--primary-dark)] transition-colors shadow-sm"
                    >
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
                            onDeleteTask={(taskId) => {
                                if (confirm("Delete task?")) deleteMutation.mutate(taskId);
                            }}
                            onSelectTask={setSelectedTask}
                            onAddTask={() => setShowCreateTask(true)}
                        />
                    ))}
                </div>
            </div>

            {/* Modals */}
            {selectedTask && (
                <TaskDetailModal
                    task={selectedTask}
                    onClose={() => setSelectedTask(null)}
                    onDelete={() => {
                        if (confirm("Delete task?")) deleteMutation.mutate(selectedTask.id);
                    }}
                />
            )}

            {showCreateTask && (
                <CreateTaskModal boardId={boardId} onClose={() => setShowCreateTask(false)} />
            )}

            {activeAttempt && (
                <LiveLogs
                    attemptId={activeAttempt.id}
                    taskTitle={activeAttempt.title}
                    onClose={() => setActiveAttempt(null)}
                />
            )}
        </div>
    );
}

function KanbanColumn({
    column,
    tasks,
    onRunTask,
    onDeleteTask,
    onSelectTask,
    onAddTask
}: {
    column: { id: string; title: string; color: string };
    tasks: Task[];
    onRunTask: (taskId: string) => void;
    onDeleteTask: (taskId: string) => void;
    onSelectTask: (task: Task) => void;
    onAddTask: () => void;
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
            <div className="flex-1 overflow-y-auto p-2 space-y-3 font-inter">
                {tasks.sort((a, b) => a.orderKey - b.orderKey).map((task) => (
                    <TaskCard
                        key={task.id}
                        task={task}
                        onRun={() => onRunTask(task.id)}
                        onDelete={() => onDeleteTask(task.id)}
                        onClick={() => onSelectTask(task)}
                    />
                ))}

                {/* Add Task Button */}
                <button
                    onClick={onAddTask}
                    className="w-full p-3 border-2 border-dashed border-[var(--border)] rounded-lg text-[var(--text-secondary)] hover:border-[var(--primary)] hover:text-[var(--primary)] hover:bg-white transition-all flex items-center justify-center gap-2"
                >
                    <Plus size={16} />
                    Add Task
                </button>
            </div>
        </div>
    );
}

function TaskCard({ task, onRun, onDelete, onClick }: { task: Task; onRun: () => void; onDelete: () => void; onClick: () => void }) {
    return (
        <div
            onClick={onClick}
            className="bg-white rounded-xl p-4 border border-[var(--border)] shadow-sm hover:shadow-md hover:border-[var(--primary)] transition-all cursor-pointer group relative"
        >
            <div className="flex items-start justify-between mb-3">
                <h4 className="font-medium text-[var(--text-primary)] group-hover:text-[var(--primary)] transition-colors line-clamp-2 pr-8">
                    {task.title}
                </h4>
                <div className="flex items-center absolute top-4 right-4 gap-1 opacity-0 group-hover:opacity-100 transition-all">
                    <button
                        onClick={(e) => { e.stopPropagation(); onDelete(); }}
                        className="p-1 rounded text-red-500 hover:bg-red-50 transition-all"
                    >
                        <Trash2 size={16} />
                    </button>
                    <button
                        onClick={(e) => { e.stopPropagation(); }}
                        className="p-1 rounded hover:bg-gray-100 transition-all"
                    >
                        <MoreHorizontal size={16} className="text-[var(--text-secondary)]" />
                    </button>
                </div>
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
    const colors = [
        'bg-gray-100 text-gray-500',
        'bg-blue-100 text-blue-700',
        'bg-emerald-100 text-emerald-700 font-bold',
        'bg-amber-100 text-amber-700 font-bold',
        'bg-rose-100 text-rose-700 font-bold shadow-sm',
        'bg-purple-100 text-purple-700 font-bold shadow-md'
    ];
    return (
        <span className={clsx('px-2 py-0.5 rounded text-[10px] uppercase tracking-tighter', colors[priority] || colors[0])}>
            Priority {priority}
        </span>
    );
}

function TaskDetailModal({ task, onClose, onDelete }: { task: Task; onClose: () => void; onDelete: () => void }) {
    return (
        <div className="fixed inset-0 bg-black/40 backdrop-blur-[2px] flex items-center justify-center z-[150]" onClick={onClose}>
            <div
                className="bg-white rounded-2xl w-full max-w-2xl max-h-[90vh] overflow-hidden shadow-2xl flex flex-col"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="p-8 border-b border-[var(--border)] flex justify-between items-start bg-slate-50/50">
                    <div>
                        <h2 className="text-2xl font-bold text-[var(--text-primary)] leading-tight">{task.title}</h2>
                        <div className="flex items-center gap-3 mt-3">
                            <span className="px-2.5 py-1 bg-white border border-[var(--border)] rounded-lg text-xs font-semibold text-[var(--text-secondary)] flex items-center gap-1.5 uppercase tracking-wide">
                                <div className={clsx("w-2 h-2 rounded-full", task.status === 'Done' ? 'bg-emerald-500' : task.status === 'InProgress' ? 'bg-amber-500' : 'bg-gray-400')} />
                                {task.status}
                            </span>
                            <PriorityBadge priority={task.priority} />
                        </div>
                    </div>
                    <button onClick={onClose} className="p-2 hover:bg-white rounded-full transition-colors border-transparent hover:border-gray-200 border">
                        <X size={20} className="text-[var(--text-secondary)]" />
                    </button>
                </div>

                <div className="p-8 flex-1 overflow-y-auto space-y-8">
                    <section>
                        <h3 className="text-sm font-bold text-slate-400 uppercase tracking-widest mb-3">Description</h3>
                        <p className="text-slate-600 leading-relaxed whitespace-pre-wrap text-lg italic">
                            {task.description || 'No detailed description provided for this orchestration step.'}
                        </p>
                    </section>

                    <section className="grid grid-cols-2 gap-8 border-t border-[var(--border)] pt-8">
                        <div className="space-y-1">
                            <span className="text-xs font-bold text-slate-400 uppercase tracking-widest">Execution Stats</span>
                            <div className="text-2xl font-mono font-bold text-slate-800">{task.attemptCount} runs</div>
                        </div>
                        <div className="space-y-1">
                            <span className="text-xs font-bold text-slate-400 uppercase tracking-widest">Metadata</span>
                            <div className="text-sm text-slate-500 font-mono">ID: {task.id.split('-')[0]}</div>
                            <div className="text-sm text-slate-500 font-mono">Created: {new Date(task.createdAt).toLocaleString()}</div>
                        </div>
                    </section>
                </div>

                <div className="p-8 bg-slate-50/80 border-t border-[var(--border)] flex gap-4">
                    <button className="flex-1 py-4 bg-[var(--success)] text-white rounded-xl font-bold hover:bg-emerald-600 active:scale-95 transition-all shadow-lg shadow-emerald-500/20 flex items-center justify-center gap-2">
                        <Play size={20} fill="white" />
                        Launch Execution
                    </button>

                    <button
                        onClick={onDelete}
                        className="px-6 py-4 bg-white border border-rose-200 text-rose-500 rounded-xl font-bold hover:bg-rose-50 active:scale-95 transition-all flex items-center gap-2"
                    >
                        <Trash2 size={20} />
                        Delete Task
                    </button>
                </div>
            </div>
        </div>
    );
}
