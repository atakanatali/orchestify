'use client';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useParams, useRouter } from 'next/navigation';
import { boardsApi, tasksApi, Task } from '@/lib/api';
import { Plus, Loader2, Play, MoreHorizontal, Trash2, X, FolderKanban, ChevronRight, Search, Users, Eye, Filter, LayoutDashboard, List, LayoutGrid, Edit3 } from 'lucide-react';
import clsx from 'clsx';
import { useState } from 'react';
import { CreateTaskModal } from '@/components/modals/CreateTaskModal';
import { EditBoardModal } from '@/components/modals/EditBoardModal';
import { EditTaskModal } from '@/components/modals/EditTaskModal';
import { TaskOrchestrationModal } from '@/components/modals/TaskOrchestrationModal';

const COLUMNS = [
    { id: 'Todo', title: 'To-do', color: 'bg-slate-400', textColor: 'text-white', borderColor: 'border-slate-400' },
    { id: 'In Progress', title: 'In Progress', color: 'bg-[#fdab3d]', textColor: 'text-white', borderColor: 'border-[#fdab3d]' },
    { id: 'Review', title: 'Review', color: 'bg-[#a25ddc]', textColor: 'text-white', borderColor: 'border-[#a25ddc]' },
    { id: 'Done', title: 'Done', color: 'bg-[#00c875]', textColor: 'text-white', borderColor: 'border-[#00c875]' },
    { id: 'Cancelled', title: 'Cancelled', color: 'bg-[#df2f4a]', textColor: 'text-white', borderColor: 'border-[#df2f4a]' },
] as const;

type Status = typeof COLUMNS[number]['id'];

export default function BoardPage() {
    const { workspaceId, boardId } = useParams<{ workspaceId: string; boardId: string }>();
    const router = useRouter();
    const queryClient = useQueryClient();
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [showCreateTask, setShowCreateTask] = useState(false);
    const [viewMode, setViewMode] = useState<'Kanban' | 'Table'>('Kanban');
    const [orchestratingTask, setOrchestratingTask] = useState<Task | null>(null);
    const [showBoardMenu, setShowBoardMenu] = useState(false);
    const [showEditBoard, setShowEditBoard] = useState(false);
    const [editingTask, setEditingTask] = useState<Task | null>(null);
    const [isTransitioning, setIsTransitioning] = useState(false);

    const { data: board, isLoading: boardLoading } = useQuery({
        queryKey: ['board', boardId],
        queryFn: () => boardsApi.get(workspaceId as string, boardId as string),
        enabled: !!boardId,
    });

    const { data: tasks, isLoading: tasksLoading } = useQuery({
        queryKey: ['tasks', boardId],
        queryFn: () => tasksApi.list(boardId as string),
        enabled: !!boardId,
    });

    const runMutation = useMutation({
        mutationFn: (taskId: string) => tasksApi.run(boardId as string, taskId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['tasks', boardId] });
        },
        onError: (error) => {
            console.error('Run task error:', error);
        }
    });

    const updateStatusMutation = useMutation({
        mutationFn: ({ taskId, status }: { taskId: string, status: Status }) =>
            tasksApi.update(boardId as string, taskId, { status }),
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['tasks', boardId] }),
    });

    const deleteMutation = useMutation({
        mutationFn: (taskId: string) => tasksApi.delete(boardId as string, taskId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['tasks', boardId] });
            setSelectedTask(null);
        },
    });

    const deleteBoardMutation = useMutation({
        mutationFn: () => boardsApi.delete(workspaceId as string, boardId as string),
        onSuccess: () => {
            router.push(`/workspaces/${workspaceId}`);
        },
        onError: (error: Error) => {
            alert(error.message || 'Cannot delete board');
        }
    });

    const handleViewChange = (newView: 'Kanban' | 'Table') => {
        if (newView === viewMode) return;
        setIsTransitioning(true);
        setTimeout(() => {
            setViewMode(newView);
            setTimeout(() => setIsTransitioning(false), 50);
        }, 200);
    };

    const handleDeleteBoard = () => {
        const activeTasks = tasks?.items?.filter(t => t.status === 'In Progress' || t.status === 'Review');
        if (activeTasks && activeTasks.length > 0) {
            alert(`Cannot delete board: ${activeTasks.length} active task(s) in progress.`);
            return;
        }
        if (confirm('Are you sure you want to delete this board?')) {
            deleteBoardMutation.mutate();
        }
    };

    // Page loading state
    if (boardLoading || tasksLoading) {
        return (
            <div className="flex items-center justify-center h-full bg-white">
                <div className="flex flex-col items-center gap-4">
                    <div className="w-12 h-12 rounded-2xl bg-indigo-100 flex items-center justify-center">
                        <Loader2 className="animate-spin text-indigo-600" size={24} />
                    </div>
                    <p className="text-slate-400 text-sm font-medium">Loading board...</p>
                </div>
            </div>
        );
    }

    const tasksByStatus = COLUMNS.reduce((acc, col) => {
        acc[col.id] = tasks?.items?.filter(t => t.status === col.id) || [];
        return acc;
    }, {} as Record<Status, Task[]>);

    return (
        <div className="h-full flex flex-col overflow-hidden bg-white">
            {/* Top Navigation / Breadcrumbs */}
            <div className="px-8 py-2 border-b border-slate-100 flex items-center justify-between shrink-0">
                <div className="flex items-center gap-2 text-[11px] font-bold text-slate-400">
                    <LayoutDashboard size={12} className="text-slate-300" />
                    <ChevronRight size={10} />
                    <FolderKanban size={12} className="text-slate-300" />
                    <span>Workspaces</span>
                    <ChevronRight size={10} />
                    <span className="text-slate-600 font-extrabold uppercase tracking-widest">{board?.board.name}</span>
                </div>
                <div className="flex items-center gap-3">
                    <div className="flex items-center gap-1.5 text-[10px] font-black text-slate-300 px-3 py-1.5 bg-slate-50 rounded-lg border border-slate-100">
                        <Users size={14} />
                        <span>/ 0 COLLABORATORS</span>
                    </div>
                    <div className="h-4 w-px bg-slate-200 mx-1" />
                    {/* Board Menu Button */}
                    <div className="relative">
                        <button
                            onClick={() => setShowBoardMenu(!showBoardMenu)}
                            className="p-2 hover:bg-slate-100 rounded-lg text-slate-400 transition-all active:scale-90"
                        >
                            <MoreHorizontal size={20} />
                        </button>
                        {showBoardMenu && (
                            <>
                                <div className="fixed inset-0 z-40" onClick={() => setShowBoardMenu(false)} />
                                <div className="absolute right-0 top-full mt-2 w-48 bg-white rounded-xl shadow-2xl border border-slate-100 py-2 z-50">
                                    <button
                                        className="w-full px-4 py-3 text-left text-sm font-medium text-slate-700 hover:bg-slate-50 flex items-center gap-3"
                                        onClick={() => { setShowBoardMenu(false); setShowEditBoard(true); }}
                                    >
                                        <Edit3 size={16} className="text-slate-400" />
                                        Edit Board
                                    </button>
                                    <button
                                        className="w-full px-4 py-3 text-left text-sm font-medium text-rose-600 hover:bg-rose-50 flex items-center gap-3"
                                        onClick={() => { setShowBoardMenu(false); handleDeleteBoard(); }}
                                    >
                                        <Trash2 size={16} />
                                        Delete Board
                                    </button>
                                </div>
                            </>
                        )}
                    </div>
                </div>
            </div>

            {/* Board Header Title Area */}
            <div className="pt-10 px-10 shrink-0">
                <div className="flex items-start justify-between mb-8">
                    <div>
                        <h1 className="text-4xl font-black text-slate-900 tracking-tight mb-2 flex items-center gap-4">
                            {board?.board.name}
                            <div className="p-1 px-3 bg-indigo-50 text-indigo-500 rounded-lg text-[10px] font-black uppercase tracking-[2px]">Orchestration</div>
                        </h1>
                        <p className="text-[14px] text-slate-400 font-medium">
                            {board?.board.description || 'Global orchestration board for project logic.'}
                        </p>
                    </div>
                </div>

                {/* Main Action Bar - Removed Timeline */}
                <div className="flex flex-col gap-6 mb-8 border-b border-slate-100 pb-1">
                    <div className="flex items-center gap-4 overflow-x-auto scrollbar-none">
                        <ViewTab
                            active={viewMode === 'Table'}
                            onClick={() => handleViewChange('Table')}
                            icon={<List size={16} />}
                            label="Main table"
                        />
                        <ViewTab
                            active={viewMode === 'Kanban'}
                            onClick={() => handleViewChange('Kanban')}
                            icon={<LayoutGrid size={16} />}
                            label="Kanban"
                        />
                        <button className="p-2 text-slate-300 hover:text-slate-600 transition-all">
                            <Plus size={16} />
                        </button>
                    </div>

                    <div className="flex items-center justify-between py-4">
                        <div className="flex items-center gap-3">
                            <button
                                onClick={() => setShowCreateTask(true)}
                                className="px-6 py-2.5 bg-indigo-600 text-white text-xs font-black uppercase tracking-[2px] rounded-xl hover:bg-indigo-700 transition-all shadow-xl shadow-indigo-500/20 active:scale-95 flex items-center gap-2"
                            >
                                <Plus size={16} />
                                New Item
                            </button>

                            <div className="relative group">
                                <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-300 group-focus-within:text-indigo-500 transition-colors" size={16} />
                                <input
                                    type="text"
                                    placeholder="Search / Filter"
                                    className="pl-12 pr-6 py-2.5 bg-slate-50 border border-slate-100 rounded-xl text-xs outline-none focus:ring-4 focus:ring-indigo-100/50 focus:border-indigo-500 transition-all min-w-[200px] placeholder:text-slate-400 font-bold"
                                />
                            </div>
                        </div>

                        <div className="flex items-center gap-4">
                            <ActionIconButton icon={<Users size={18} />} />
                            <ActionIconButton icon={<Eye size={18} />} />
                            <ActionIconButton icon={<Filter size={18} />} />
                        </div>
                    </div>
                </div>
            </div>

            {/* Content Area with Transition */}
            <div className={clsx(
                "flex-1 overflow-auto bg-white px-10 pb-10 transition-all duration-300",
                isTransitioning ? "opacity-0 translate-y-4" : "opacity-100 translate-y-0"
            )}>
                {viewMode === 'Kanban' ? (
                    <div className="flex gap-8 h-full min-w-max">
                        {COLUMNS.map((column) => (
                            <KanbanColumn
                                key={column.id}
                                column={column}
                                tasks={tasksByStatus[column.id]}
                                onRunTask={(taskId) => {
                                    const task = tasks?.items?.find(t => t.id === taskId);
                                    if (task) setOrchestratingTask(task);
                                }}
                                onDeleteTask={(taskId) => {
                                    if (confirm("Delete task?")) deleteMutation.mutate(taskId);
                                }}
                                onSelectTask={setSelectedTask}
                                onAddTask={() => setShowCreateTask(true)}
                                onDropTask={(taskId, newStatus) => updateStatusMutation.mutate({ taskId, status: newStatus })}
                            />
                        ))}
                    </div>
                ) : (
                    <div className="space-y-12">
                        {COLUMNS.map((column) => (
                            <TableViewSection
                                key={column.id}
                                column={column}
                                tasks={tasksByStatus[column.id]}
                                onSelectTask={setSelectedTask}
                                onRunTask={(taskId) => {
                                    const task = tasks?.items?.find(t => t.id === taskId);
                                    if (task) setOrchestratingTask(task);
                                }}
                                onUpdateStatus={(taskId, status) => updateStatusMutation.mutate({ taskId, status })}
                            />
                        ))}
                    </div>
                )}
            </div>

            {orchestratingTask && (
                <TaskOrchestrationModal
                    task={orchestratingTask}
                    boardId={boardId as string}
                    onClose={() => setOrchestratingTask(null)}
                />
            )}

            {showEditBoard && board && (
                <EditBoardModal
                    workspaceId={workspaceId as string}
                    board={board.board}
                    onClose={() => setShowEditBoard(false)}
                />
            )}

            {editingTask && (
                <EditTaskModal
                    boardId={boardId as string}
                    task={editingTask}
                    onClose={() => setEditingTask(null)}
                />
            )}
        </div>
    );
}

function ViewTab({ active, onClick, icon, label }: { active: boolean, onClick: () => void, icon: React.ReactNode, label: string }) {
    return (
        <button
            onClick={onClick}
            className={clsx(
                "flex items-center gap-2 px-4 py-2 border-b-2 transition-all font-bold text-xs whitespace-nowrap",
                active ? "border-indigo-600 text-indigo-600" : "border-transparent text-slate-400 hover:text-slate-600 hover:bg-slate-50 rounded-t-lg"
            )}
        >
            {icon}
            {label}
        </button>
    );
}

function ActionIconButton({ icon }: { icon: React.ReactNode }) {
    return (
        <button className="p-2.5 text-slate-300 hover:text-slate-600 hover:bg-slate-50 rounded-xl transition-all active:scale-90 border border-transparent hover:border-slate-100">
            {icon}
        </button>
    );
}

function KanbanColumn({
    column,
    tasks,
    onRunTask,
    onDeleteTask,
    onSelectTask,
    onAddTask,
    onDropTask
}: {
    column: typeof COLUMNS[number];
    tasks: Task[];
    onRunTask: (taskId: string) => void;
    onDeleteTask: (taskId: string) => void;
    onSelectTask: (task: Task) => void;
    onAddTask: () => void;
    onDropTask: (taskId: string, newStatus: Status) => void;
}) {
    const [isDragOver, setIsDragOver] = useState(false);

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault();
        setIsDragOver(true);
    };

    const handleDragLeave = () => {
        setIsDragOver(false);
    };

    const handleDrop = (e: React.DragEvent) => {
        e.preventDefault();
        setIsDragOver(false);
        const taskId = e.dataTransfer.getData('taskId');
        if (taskId) {
            onDropTask(taskId, column.id as Status);
        }
    };

    return (
        <div
            className={clsx(
                "w-[300px] flex-shrink-0 flex flex-col bg-slate-50/50 rounded-[32px] overflow-hidden border-2 shadow-sm transition-all",
                isDragOver ? "border-indigo-500 bg-indigo-50/50 scale-[1.02]" : "border-slate-100"
            )}
            onDragOver={handleDragOver}
            onDragLeave={handleDragLeave}
            onDrop={handleDrop}
        >
            {/* Column Header */}
            <div className={clsx("px-6 py-5 font-black text-[11px] tracking-[3px] shadow-sm uppercase text-center border-b-4", column.color, column.textColor, column.borderColor)}>
                {column.title} / {tasks.length}
            </div>

            {/* Tasks */}
            <div className="flex-1 overflow-y-auto p-6 space-y-6">
                {tasks?.sort((a, b) => a.orderKey - b.orderKey).map((task) => (
                    <div
                        key={task.id}
                        draggable
                        onDragStart={(e) => {
                            e.dataTransfer.setData('taskId', task.id);
                            e.dataTransfer.effectAllowed = 'move';
                        }}
                        onClick={() => onSelectTask(task)}
                        className="bg-white rounded-[24px] p-6 border-2 border-transparent shadow-sm hover:shadow-2xl hover:border-indigo-500 transition-all cursor-grab group relative active:scale-[0.98] active:cursor-grabbing"
                    >
                        <h4 className="text-[15px] font-bold text-slate-800 leading-tight tracking-tight line-clamp-3 mb-6">
                            {task.title}
                        </h4>

                        <div className="flex items-center justify-between">
                            <div className="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center text-[11px] font-black text-indigo-600">
                                O
                            </div>

                            <div className="flex items-center gap-2 opacity-0 group-hover:opacity-100 transition-all">
                                <button
                                    onClick={(e) => { e.stopPropagation(); onRunTask(task.id); }}
                                    className="p-2.5 bg-emerald-500 text-white rounded-xl hover:bg-emerald-600 shadow-lg shadow-emerald-500/30 active:scale-90"
                                >
                                    <Play size={10} fill="white" />
                                </button>
                                <button
                                    onClick={(e) => { e.stopPropagation(); onDeleteTask(task.id); }}
                                    className="p-2.5 bg-rose-50 text-rose-500 hover:bg-rose-500 hover:text-white rounded-xl active:scale-90 transition-all border border-rose-100"
                                >
                                    <Trash2 size={12} />
                                </button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            {/* Footer Add Button */}
            <div className="p-4 bg-white mt-auto border-t border-slate-100">
                <button
                    onClick={onAddTask}
                    className="flex items-center gap-2 text-xs font-black text-white bg-indigo-600 hover:bg-indigo-700 transition-all px-4 py-4 rounded-2xl shadow-xl shadow-indigo-500/20 active:scale-95 w-full justify-center uppercase tracking-[2px]"
                >
                    <Plus size={18} />
                    Add Task
                </button>
            </div>
        </div>
    );
}

function TableViewSection({
    column,
    tasks,
    onSelectTask,
    onRunTask,
    onUpdateStatus
}: {
    column: typeof COLUMNS[number],
    tasks: Task[],
    onSelectTask: (task: Task) => void,
    onRunTask: (taskId: string) => void,
    onUpdateStatus: (taskId: string, status: Status) => void
}) {
    const [openStatusDropdown, setOpenStatusDropdown] = useState<string | null>(null);

    return (
        <div className="space-y-4">
            <div className="flex items-center gap-4">
                <div className={clsx("w-2 h-8 rounded-full shadow-sm", column.color)} />
                <h3 className={clsx("text-lg font-black tracking-tight", "text-slate-800")}>
                    {column.title}
                </h3>
                <span className="px-2 py-0.5 bg-slate-50 rounded text-[10px] font-black text-slate-400 border border-slate-100 uppercase tracking-widest">
                    {tasks.length} Items
                </span>
            </div>

            <div className="bg-white rounded-3xl border border-slate-100 shadow-sm overflow-visible">
                <table className="w-full text-left border-collapse">
                    <thead>
                        <tr className="bg-slate-50/50 border-b border-slate-100">
                            <th className="w-[50%] px-8 py-5 text-[10px] font-black text-slate-400 uppercase tracking-[2px]">Item Name</th>
                            <th className="px-6 py-5 text-[10px] font-black text-slate-400 uppercase tracking-[2px] text-center">Assigned</th>
                            <th className="px-6 py-5 text-[10px] font-black text-slate-400 uppercase tracking-[2px] text-center">Status</th>
                            <th className="px-6 py-5 text-[10px] font-black text-slate-400 uppercase tracking-[2px] text-right pr-10">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-50">
                        {tasks.length === 0 ? (
                            <tr>
                                <td colSpan={4} className="px-8 py-10 text-center text-slate-400 font-medium italic text-sm">
                                    No tasks in this segment.
                                </td>
                            </tr>
                        ) : (
                            tasks.map(task => (
                                <tr key={task.id} className="hover:bg-slate-50/50 group transition-colors cursor-pointer" onClick={() => onSelectTask(task)}>
                                    <td className="px-8 py-5 border-l-4 border-transparent group-hover:border-indigo-500">
                                        <div className="font-bold text-slate-800 text-[15px]">{task.title}</div>
                                    </td>
                                    <td className="px-6 py-5">
                                        <div className="flex justify-center">
                                            <div className="w-8 h-8 rounded-full bg-indigo-100 border-2 border-white shadow-sm flex items-center justify-center text-[11px] font-black text-indigo-600">O</div>
                                        </div>
                                    </td>
                                    <td className="px-6 py-5">
                                        <div className="relative flex justify-center">
                                            <button
                                                onClick={(e) => {
                                                    e.stopPropagation();
                                                    setOpenStatusDropdown(openStatusDropdown === task.id ? null : task.id);
                                                }}
                                                className={clsx(
                                                    "px-3 py-1.5 rounded-lg text-white font-black text-[10px] text-center uppercase tracking-wider shadow-sm transition-all hover:scale-105",
                                                    column.color
                                                )}
                                            >
                                                {column.title}
                                            </button>
                                            {openStatusDropdown === task.id && (
                                                <>
                                                    <div className="fixed inset-0 z-[100]" onClick={(e) => { e.stopPropagation(); setOpenStatusDropdown(null); }} />
                                                    <div className="absolute top-full left-1/2 -translate-x-1/2 mt-2 z-[101] bg-white shadow-2xl rounded-xl border border-slate-100 p-2 min-w-[130px]">
                                                        {COLUMNS.map(c => (
                                                            <button
                                                                key={c.id}
                                                                onClick={(e) => {
                                                                    e.stopPropagation();
                                                                    onUpdateStatus(task.id, c.id);
                                                                    setOpenStatusDropdown(null);
                                                                }}
                                                                className={clsx(
                                                                    "w-full px-3 py-2 rounded-lg text-[10px] font-black uppercase tracking-wider text-left mb-1 last:mb-0 transition-all hover:scale-[1.02]",
                                                                    c.color, c.textColor
                                                                )}
                                                            >
                                                                {c.title}
                                                            </button>
                                                        ))}
                                                    </div>
                                                </>
                                            )}
                                        </div>
                                    </td>
                                    <td className="px-6 py-5 text-right pr-10">
                                        <div className="flex items-center justify-end gap-3 opacity-0 group-hover:opacity-100 transition-all">
                                            <button
                                                onClick={(e) => { e.stopPropagation(); onRunTask(task.id); }}
                                                className="p-2 bg-emerald-500 text-white rounded-lg shadow-lg shadow-emerald-500/20 active:scale-90"
                                            >
                                                <Play size={12} fill="currentColor" />
                                            </button>
                                            <button className="p-2 bg-slate-100 text-slate-400 rounded-lg hover:bg-slate-200 active:scale-90">
                                                <MoreHorizontal size={16} />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
