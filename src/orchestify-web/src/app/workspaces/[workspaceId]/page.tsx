'use client';

import { useQuery } from '@tanstack/react-query';
import { useParams } from 'next/navigation';
import Link from 'next/link';
import { workspacesApi, boardsApi, Board } from '@/lib/api';
import { Plus, LayoutGrid, Loader2, MoreHorizontal, ListTodo, CheckCircle2, Trash2, FolderKanban, ChevronRight, GitBranch } from 'lucide-react';
import { useState } from 'react';
import { CreateBoardModal } from '@/components/modals/CreateBoardModal';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import clsx from 'clsx';

export default function WorkspacePage() {
    const { workspaceId } = useParams<{ workspaceId: string }>();
    const [showCreateBoard, setShowCreateBoard] = useState(false);

    const { data: workspace } = useQuery({
        queryKey: ['workspace', workspaceId],
        queryFn: () => workspacesApi.get(workspaceId),
        enabled: !!workspaceId,
    });

    const { data: boards, isLoading } = useQuery({
        queryKey: ['boards', workspaceId],
        queryFn: () => boardsApi.list(workspaceId),
        enabled: !!workspaceId,
    });

    if (isLoading) {
        return (
            <div className="flex items-center justify-center h-full">
                <Loader2 className="animate-spin text-indigo-600" size={32} />
            </div>
        );
    }

    const repoName = workspace?.workspace?.repositoryPath.split('/').pop();

    return (
        <div className="p-10 max-w-7xl mx-auto">
            {/* Header Area */}
            <div className="flex flex-col md:flex-row md:items-end justify-between gap-6 mb-12">
                <div className="space-y-4">
                    <div className="flex items-center gap-3 text-slate-400 text-[11px] font-black uppercase tracking-[2px]">
                        <FolderKanban size={16} />
                        <span>Workspace</span>
                        <ChevronRight size={12} />
                        <span className="text-indigo-600 uppercase tracking-[4px]">{workspace?.workspace?.name}</span>
                    </div>
                    <div>
                        <h1 className="text-4xl font-black text-slate-900 tracking-tight leading-none mb-4">
                            {workspace?.workspace?.name}
                        </h1>
                        <div className="flex items-center gap-6">
                            <div className="flex items-center gap-2 px-3 py-1.5 bg-slate-100 rounded-full text-xs font-bold text-slate-500 border border-slate-200">
                                <FolderKanban size={14} />
                                <span>/repos/{repoName}</span>
                            </div>
                            <div className="flex items-center gap-2 px-3 py-1.5 bg-indigo-50 rounded-full text-xs font-bold text-indigo-600 border border-indigo-100">
                                <GitBranch size={14} />
                                <span>{workspace?.workspace?.defaultBranch}</span>
                            </div>
                        </div>
                    </div>
                </div>

                <button
                    onClick={() => setShowCreateBoard(true)}
                    className="flex items-center gap-3 px-8 py-4 bg-indigo-600 text-white rounded-2xl font-black hover:bg-indigo-700 transition-all shadow-2xl shadow-indigo-500/20 active:scale-95 text-lg"
                >
                    <Plus size={24} />
                    New Board
                </button>
            </div>

            {/* Boards Section */}
            <div className="mb-6 flex items-center justify-between">
                <h2 className="text-sm font-black text-slate-400 uppercase tracking-[4px]">Active Boards</h2>
                <div className="h-px flex-1 bg-slate-100 mx-6 opacity-50" />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                {boards?.items?.map((board) => (
                    <BoardCard key={board.id} board={board} workspaceId={workspaceId} />
                ))}

                {/* Create New Board Card */}
                <button
                    onClick={() => setShowCreateBoard(true)}
                    className="border-4 border-dashed border-slate-100 rounded-[32px] p-10 flex flex-col items-center justify-center gap-4 hover:border-indigo-400 hover:bg-indigo-50/30 transition-all group min-h-[220px]"
                >
                    <div className="w-16 h-16 rounded-3xl bg-slate-50 flex items-center justify-center group-hover:bg-indigo-600 group-hover:scale-110 group-hover:rotate-90 transition-all duration-300">
                        <Plus size={32} className="text-slate-300 group-hover:text-white" />
                    </div>
                    <span className="text-sm font-black text-slate-400 uppercase tracking-[2px] group-hover:text-indigo-600">
                        Create New Board
                    </span>
                </button>
            </div>

            {/* Modals */}
            {showCreateBoard && (
                <CreateBoardModal workspaceId={workspaceId} onClose={() => setShowCreateBoard(false)} />
            )}
        </div>
    );
}

function BoardCard({ board, workspaceId }: { board: Board; workspaceId: string }) {
    const queryClient = useQueryClient();
    const deleteMutation = useMutation({
        mutationFn: () => boardsApi.delete(workspaceId, board.id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['boards', workspaceId] });
        }
    });

    const handleDelete = (e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        if (confirm(`Are you sure you want to delete board "${board.name}"?`)) {
            deleteMutation.mutate();
        }
    };

    const progress = board.totalTasks > 0
        ? Math.round((board.completedTasks / board.totalTasks) * 100)
        : 0;

    return (
        <Link
            href={`/workspaces/${workspaceId}/boards/${board.id}`}
            className="bg-white rounded-[32px] border border-slate-100 p-8 hover:shadow-[0_20px_50px_rgba(0,0,0,0.06)] hover:-translate-y-2 transition-all group relative overflow-hidden"
        >
            <div className="flex items-start justify-between mb-8">
                <div className="w-12 h-12 rounded-2xl bg-indigo-50 flex items-center justify-center text-indigo-600 group-hover:bg-indigo-600 group-hover:text-white transition-all duration-300">
                    <LayoutGrid size={24} />
                </div>
                <div className="flex items-center gap-1 opacity-100 group-hover:opacity-100 transition-all">
                    <button
                        onClick={handleDelete}
                        className="p-2.5 rounded-xl text-slate-300 hover:bg-rose-50 hover:text-rose-500 transition-all disabled:opacity-50 active:scale-90"
                        disabled={deleteMutation.isPending}
                    >
                        <Trash2 size={18} />
                    </button>
                    <button
                        onClick={(e) => { e.preventDefault(); e.stopPropagation(); }}
                        className="p-2.5 rounded-xl text-slate-300 hover:bg-slate-50 hover:text-slate-900 transition-all active:scale-90"
                    >
                        <MoreHorizontal size={18} />
                    </button>
                </div>
            </div>

            <h3 className="text-xl font-black text-slate-900 mb-2 group-hover:text-indigo-600 transition-colors tracking-tight">
                {board.name}
            </h3>
            <p className="text-sm text-slate-400 font-medium line-clamp-2 mb-8 h-10 overflow-hidden leading-relaxed">
                {board.description || 'Global orchestration board for project logic.'}
            </p>

            {/* Progress Bar Section */}
            <div className="space-y-3">
                <div className="flex justify-between items-end">
                    <div className="flex items-center gap-3">
                        <div className="flex items-center gap-1.5 text-slate-900 font-black text-sm">
                            <ListTodo size={14} className="text-indigo-500" />
                            <span>{board.totalTasks}</span>
                        </div>
                        <div className="h-3 w-px bg-slate-100" />
                        <div className="flex items-center gap-1.5 text-emerald-600 font-black text-sm">
                            <CheckCircle2 size={14} />
                            <span>{board.completedTasks}</span>
                        </div>
                    </div>
                    <span className="text-[10px] font-black text-slate-300 uppercase tracking-widest">{progress}% COMPLETED</span>
                </div>
                <div className="h-2.5 bg-slate-50 rounded-full overflow-hidden border border-slate-100">
                    <div
                        className="h-full bg-gradient-to-r from-indigo-500 to-indigo-600 rounded-full transition-all duration-700 ease-out shadow-[0_0_12px_rgba(79,70,229,0.3)]"
                        style={{ width: `${progress}%` }}
                    />
                </div>
            </div>

            {/* Background Accent */}
            <div className="absolute -bottom-8 -right-8 w-24 h-24 bg-indigo-500/5 rounded-full blur-2xl group-hover:bg-indigo-500/10 transition-all duration-500" />
        </Link>
    );
}
