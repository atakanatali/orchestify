'use client';

import { useQuery } from '@tanstack/react-query';
import { useParams } from 'next/navigation';
import Link from 'next/link';
import { workspacesApi, boardsApi, Board } from '@/lib/api';
import { Plus, LayoutGrid, Loader2, MoreHorizontal, ListTodo, CheckCircle2, Trash2 } from 'lucide-react';
import { useState } from 'react';
import { CreateBoardModal } from '@/components/modals/CreateBoardModal';
import { useMutation, useQueryClient } from '@tanstack/react-query';

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
                <Loader2 className="animate-spin text-[var(--primary)]" size={32} />
            </div>
        );
    }

    return (
        <div className="p-8">
            {/* Header */}
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-2xl font-bold text-[var(--text-primary)]">
                        {workspace?.workspace?.name}
                    </h1>
                    <p className="text-[var(--text-secondary)]">
                        {workspace?.workspace?.repositoryPath}
                    </p>
                </div>
                <button
                    onClick={() => setShowCreateBoard(true)}
                    className="flex items-center gap-2 px-4 py-2.5 bg-[var(--primary)] text-white rounded-lg hover:bg-[var(--primary-dark)] transition-colors shadow-sm"
                >
                    <Plus size={18} />
                    New Board
                </button>
            </div>

            {/* Boards Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5">
                {boards?.items?.map((board) => (
                    <BoardCard key={board.id} board={board} workspaceId={workspaceId} />
                ))}

                {/* Add Board Placeholder */}
                <button
                    onClick={() => setShowCreateBoard(true)}
                    className="border-2 border-dashed border-[var(--border)] rounded-xl p-6 flex flex-col items-center justify-center gap-3 hover:border-[var(--primary)] hover:bg-[var(--primary)] hover:bg-opacity-5 transition-all min-h-[200px] group"
                >
                    <div className="p-3 rounded-full bg-gray-100 group-hover:bg-[var(--primary)] group-hover:bg-opacity-10 transition-colors">
                        <Plus size={24} className="text-[var(--text-secondary)] group-hover:text-[var(--primary)]" />
                    </div>
                    <span className="font-medium text-[var(--text-secondary)] group-hover:text-[var(--primary)]">
                        Add Board
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
            className="bg-white rounded-xl border border-[var(--border)] p-5 hover:shadow-lg hover:border-[var(--primary)] transition-all group relative"
        >
            <div className="flex items-start justify-between mb-4">
                <div className="p-2 rounded-lg bg-[var(--primary)] bg-opacity-10">
                    <LayoutGrid size={20} className="text-[var(--primary)]" />
                </div>
                <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-all">
                    <button
                        onClick={handleDelete}
                        className="p-1.5 rounded-lg text-red-500 hover:bg-red-50 transition-all disabled:opacity-50"
                        disabled={deleteMutation.isPending}
                    >
                        <Trash2 size={18} />
                    </button>
                    <button
                        onClick={(e) => { e.preventDefault(); }}
                        className="p-1.5 rounded-lg hover:bg-gray-100 transition-all"
                    >
                        <MoreHorizontal size={18} className="text-[var(--text-secondary)]" />
                    </button>
                </div>
            </div>

            <h3 className="font-semibold text-[var(--text-primary)] mb-1 group-hover:text-[var(--primary)] transition-colors">
                {board.name}
            </h3>
            <p className="text-sm text-[var(--text-secondary)] line-clamp-2 mb-4">
                {board.description || 'No description'}
            </p>

            {/* Progress */}
            <div className="mb-3">
                <div className="flex justify-between text-xs mb-1">
                    <span className="text-[var(--text-secondary)]">Progress</span>
                    <span className="font-medium">{progress}%</span>
                </div>
                <div className="h-1.5 bg-gray-100 rounded-full overflow-hidden">
                    <div
                        className="h-full bg-[var(--success)] rounded-full transition-all"
                        style={{ width: `${progress}%` }}
                    />
                </div>
            </div>

            {/* Stats */}
            <div className="flex items-center gap-4 text-sm">
                <div className="flex items-center gap-1 text-[var(--text-secondary)]">
                    <ListTodo size={14} />
                    <span>{board.totalTasks} tasks</span>
                </div>
                <div className="flex items-center gap-1 text-[var(--success)]">
                    <CheckCircle2 size={14} />
                    <span>{board.completedTasks} done</span>
                </div>
            </div>
        </Link>
    );
}
