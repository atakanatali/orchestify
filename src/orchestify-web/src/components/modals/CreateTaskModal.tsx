'use client';

import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { tasksApi } from '@/lib/api';
import { X, Loader2, ChevronRight, ListTodo, AlignLeft, Plus } from 'lucide-react';

export function CreateTaskModal({ boardId, onClose }: { boardId: string; onClose: () => void }) {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');

    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: () => tasksApi.create(boardId, {
            title,
            description,
            priority: 0 // Default to lowest priority since we are removing selection
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
        <div className="fixed inset-0 bg-slate-900/70 flex items-center justify-center z-[200] backdrop-blur-[8px] p-4" onClick={onClose}>
            <div
                className="bg-white rounded-[32px] w-full max-w-xl shadow-[0_30px_60px_rgba(15,23,42,0.3)] overflow-hidden scale-in-center animate-in fade-in zoom-in duration-300 border border-slate-200"
                onClick={(e) => e.stopPropagation()}
            >
                {/* Header */}
                <div className="flex items-center justify-between p-10 border-b border-slate-100 bg-gradient-to-br from-slate-50 to-white">
                    <div className="flex items-center gap-5">
                        <div className="w-14 h-14 bg-indigo-600 rounded-2xl flex items-center justify-center text-white shadow-xl shadow-indigo-500/20">
                            <Plus size={32} />
                        </div>
                        <div>
                            <h2 className="text-3xl font-black text-slate-900 tracking-tight leading-none mb-2">Create Task</h2>
                            <p className="text-slate-400 font-medium">Define a new orchestration step</p>
                        </div>
                    </div>
                    <button onClick={onClose} className="p-3 hover:bg-slate-100 rounded-full transition-all border border-transparent hover:border-slate-200 group">
                        <X size={24} className="text-slate-300 group-hover:text-slate-900" />
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-10 space-y-8">
                    {/* Task Title */}
                    <div className="space-y-2">
                        <label className="text-[10px] font-black text-slate-400 uppercase tracking-[2px] ml-1">
                            Task Title
                        </label>
                        <div className="relative">
                            <input
                                autoFocus
                                type="text"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="e.g. Run Unit Tests"
                                className="w-full pl-14 pr-6 py-5 bg-slate-50 border border-slate-100 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all text-slate-700 font-bold text-lg placeholder:text-slate-300 shadow-sm"
                                required
                            />
                            <ListTodo size={24} className="absolute left-6 top-1/2 -translate-y-1/2 text-slate-300" />
                        </div>
                    </div>

                    {/* Task Description */}
                    <div className="space-y-2">
                        <label className="text-[10px] font-black text-slate-400 uppercase tracking-[2px] ml-1">
                            Description / Logic Definition
                        </label>
                        <div className="relative">
                            <textarea
                                value={description}
                                onChange={(e) => setDescription(e.target.value)}
                                placeholder="Describe the orchestration logic..."
                                className="w-full pl-14 pr-6 py-5 bg-slate-50 border border-slate-100 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all text-slate-700 font-medium text-sm placeholder:text-slate-300 shadow-sm min-h-[120px] resize-none"
                            />
                            <AlignLeft size={24} className="absolute left-6 top-6 text-slate-300" />
                        </div>
                    </div>

                    {/* Actions */}
                    <div className="pt-6 flex gap-5">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-8 py-5 text-slate-500 rounded-2xl font-black uppercase tracking-[2px] text-xs hover:bg-slate-50 transition-all active:scale-95"
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={mutation.isPending || !title}
                            className="flex-2 px-10 py-5 bg-indigo-600 text-white rounded-2xl font-black hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-2xl shadow-indigo-500/30 active:scale-95 flex items-center justify-center gap-4 text-lg"
                        >
                            {mutation.isPending ? <Loader2 size={24} className="animate-spin" /> : (
                                <>
                                    <span>Create Task</span>
                                    <ChevronRight size={20} />
                                </>
                            )}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

