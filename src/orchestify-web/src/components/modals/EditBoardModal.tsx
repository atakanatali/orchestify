'use client';

import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { boardsApi } from '@/lib/api';
import { X, Loader2, Save, Edit3 } from 'lucide-react';

interface EditBoardModalProps {
    workspaceId: string;
    board: { id: string; name: string; description: string };
    onClose: () => void;
}

export function EditBoardModal({ workspaceId, board, onClose }: EditBoardModalProps) {
    const [name, setName] = useState(board.name);
    const [description, setDescription] = useState(board.description || '');
    const queryClient = useQueryClient();

    const mutation = useMutation({
        mutationFn: () => boardsApi.update(workspaceId, board.id, { name, description }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['boards', workspaceId] });
            queryClient.invalidateQueries({ queryKey: ['board', board.id] });
            onClose();
        }
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!name) return;
        mutation.mutate();
    };

    return (
        <div className="fixed inset-0 bg-slate-900/70 flex items-center justify-center z-[200] backdrop-blur-[8px] p-4" onClick={onClose}>
            <div
                className="bg-white rounded-[32px] w-full max-w-md shadow-[0_30px_60px_rgba(15,23,42,0.3)] overflow-hidden animate-in fade-in zoom-in duration-300 border border-slate-200"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between p-8 border-b border-slate-100 bg-gradient-to-br from-slate-50 to-white">
                    <div className="flex items-center gap-4">
                        <div className="w-12 h-12 bg-indigo-600 rounded-2xl flex items-center justify-center text-white shadow-xl shadow-indigo-500/20">
                            <Edit3 size={24} />
                        </div>
                        <h2 className="text-2xl font-black text-slate-900 tracking-tight">Edit Board</h2>
                    </div>
                    <button onClick={onClose} className="p-3 hover:bg-slate-100 rounded-full transition-all">
                        <X size={20} className="text-slate-400" />
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-8 space-y-6">
                    <div className="space-y-2">
                        <label className="text-[10px] font-black text-slate-400 uppercase tracking-[2px]">Board Name</label>
                        <input
                            autoFocus
                            type="text"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            className="w-full px-5 py-4 bg-slate-50 border border-slate-100 rounded-xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all text-slate-700 font-bold"
                            required
                        />
                    </div>

                    <div className="space-y-2">
                        <label className="text-[10px] font-black text-slate-400 uppercase tracking-[2px]">Description</label>
                        <textarea
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            className="w-full px-5 py-4 bg-slate-50 border border-slate-100 rounded-xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all text-slate-700 font-medium resize-none h-24"
                        />
                    </div>

                    <div className="pt-4 flex gap-4">
                        <button type="button" onClick={onClose} className="flex-1 px-6 py-4 text-slate-500 rounded-xl font-black uppercase tracking-[2px] text-xs hover:bg-slate-50 transition-all">
                            Cancel
                        </button>
                        <button
                            type="submit"
                            disabled={mutation.isPending || !name}
                            className="flex-1 px-6 py-4 bg-indigo-600 text-white rounded-xl font-black hover:bg-indigo-700 disabled:opacity-50 transition-all shadow-xl shadow-indigo-500/30 flex items-center justify-center gap-2 text-sm uppercase tracking-widest"
                        >
                            {mutation.isPending ? <Loader2 size={18} className="animate-spin" /> : <Save size={18} />}
                            Save
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
