'use client';

import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { tasksApi } from '@/lib/api';
import { X, Loader2 } from 'lucide-react';

export function CreateTaskModal({
    boardId,
    onClose
}: {
    boardId: string;
    onClose: () => void
}) {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [priority, setPriority] = useState(1);

    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: () => tasksApi.create(boardId, {
            title,
            description,
            priority
        }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['tasks', boardId] });
            onClose();
        }
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!title) return;
        mutation.mutate();
    };

    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[100] backdrop-blur-sm" onClick={onClose}>
            <div
                className="bg-white rounded-2xl w-full max-w-md shadow-2xl overflow-hidden"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between p-6 border-b border-[var(--border)]">
                    <h2 className="text-xl font-bold text-[var(--text-primary)]">New Task</h2>
                    <button onClick={onClose} className="p-2 hover:bg-gray-100 rounded-full transition-colors">
                        <X size={20} className="text-[var(--text-secondary)]" />
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-6 space-y-4">
                    <div>
                        <label className="block text-sm font-semibold text-[var(--text-secondary)] mb-1.5">
                            Task Title
                        </label>
                        <input
                            autoFocus
                            type="text"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            placeholder="e.g. Implement Auth"
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
                            placeholder="What needs to be done?"
                            rows={3}
                            className="w-full px-4 py-2.5 bg-gray-50 border border-[var(--border)] rounded-xl focus:ring-2 focus:ring-[var(--primary)] focus:bg-white outline-none resize-none transition-all"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-semibold text-[var(--text-secondary)] mb-1.5">
                            Priority
                        </label>
                        <div className="flex gap-2">
                            {[1, 2, 3, 4, 5].map((p) => (
                                <button
                                    key={p}
                                    type="button"
                                    onClick={() => setPriority(p)}
                                    className={`flex-1 py-2 rounded-lg text-sm font-bold transition-all ${priority === p
                                            ? 'bg-[var(--primary)] text-white scale-105 shadow-md'
                                            : 'bg-gray-100 text-gray-500 hover:bg-gray-200'
                                        }`}
                                >
                                    P{p}
                                </button>
                            ))}
                        </div>
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
                            disabled={mutation.isPending || !title}
                            className="flex-1 px-4 py-2.5 bg-[var(--primary)] text-white rounded-xl font-semibold hover:bg-[var(--primary-dark)] disabled:opacity-50 disabled:cursor-not-allowed transition-all flex items-center justify-center gap-2"
                        >
                            {mutation.isPending ? <Loader2 size={20} className="animate-spin" /> : 'Create Task'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
