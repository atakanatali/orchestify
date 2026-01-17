'use client';

import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { boardsApi } from '@/lib/api';
import { X, Loader2 } from 'lucide-react';

export function CreateBoardModal({
    workspaceId,
    onClose
}: {
    workspaceId: string;
    onClose: () => void
}) {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');

    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: () => boardsApi.create(workspaceId, {
            name,
            description
        }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['boards', workspaceId] });
            onClose();
        }
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!name) return;
        mutation.mutate();
    };

    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[100] backdrop-blur-sm" onClick={onClose}>
            <div
                className="bg-white rounded-2xl w-full max-w-md shadow-2xl overflow-hidden"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between p-6 border-b border-[var(--border)]">
                    <h2 className="text-xl font-bold text-[var(--text-primary)]">New Board</h2>
                    <button onClick={onClose} className="p-2 hover:bg-gray-100 rounded-full transition-colors">
                        <X size={20} className="text-[var(--text-secondary)]" />
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-6 space-y-4">
                    <div>
                        <label className="block text-sm font-semibold text-[var(--text-secondary)] mb-1.5">
                            Board Name
                        </label>
                        <input
                            autoFocus
                            type="text"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            placeholder="e.g. Backend Development"
                            className="w-full px-4 py-2.5 bg-gray-50 border border-[var(--border)] rounded-xl focus:ring-2 focus:ring-[var(--primary)] focus:bg-white outline-none transition-all"
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-semibold text-[var(--text-secondary)] mb-1.5">
                            Description
                        </label>
                        <textarea
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            placeholder="What is this board for?"
                            rows={3}
                            className="w-full px-4 py-2.5 bg-gray-50 border border-[var(--border)] rounded-xl focus:ring-2 focus:ring-[var(--primary)] focus:bg-white outline-none resize-none transition-all"
                        />
                    </div>

                    <div className="pt-4 flex gap-3">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2.5 border border-[var(--border)] text-gray-600 rounded-xl font-semibold hover:bg-gray-50 transition-all"
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={mutation.isPending || !name}
                            className="flex-1 px-4 py-2.5 bg-[var(--primary)] text-white rounded-xl font-semibold hover:bg-[var(--primary-dark)] disabled:opacity-50 disabled:cursor-not-allowed transition-all flex items-center justify-center gap-2"
                        >
                            {mutation.isPending ? <Loader2 size={20} className="animate-spin" /> : 'Create Board'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
