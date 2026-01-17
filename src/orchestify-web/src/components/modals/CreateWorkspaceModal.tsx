'use client';

import { useState, useEffect } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { workspacesApi, discoveryApi, RepositoryInfo } from '@/lib/api';
import { X, Loader2, Info, ChevronRight, GitBranch, FolderCheck, AlertCircle } from 'lucide-react';
import clsx from 'clsx';

export function CreateWorkspaceModal({ onClose }: { onClose: () => void }) {
    const [name, setName] = useState('');
    const [selectedRepo, setSelectedRepo] = useState<RepositoryInfo | null>(null);
    const [branch, setBranch] = useState('');

    const queryClient = useQueryClient();

    // Fetch available repositories
    const { data: repos, isLoading: loadingRepos, error: repoError } = useQuery({
        queryKey: ['discovery-repos'],
        queryFn: discoveryApi.listRepos,
    });

    // Fetch branches when repo is selected
    const { data: branches, isLoading: loadingBranches } = useQuery({
        queryKey: ['discovery-branches', selectedRepo?.name],
        queryFn: () => discoveryApi.listBranches(selectedRepo!.name),
        enabled: !!selectedRepo,
    });

    // Auto-select first branch when branches are loaded
    useEffect(() => {
        if (branches && branches.length > 0 && !branch) {
            const mainBranch = branches.find(b => b === 'main' || b === 'master') || branches[0];
            setBranch(mainBranch);
        }
    }, [branches, branch]);

    const mutation = useMutation({
        mutationFn: () => workspacesApi.create({
            name,
            repositoryPath: selectedRepo!.fullPath,
            defaultBranch: branch
        }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['workspaces'] });
            onClose();
        }
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!name || !selectedRepo || !branch) return;
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
                            <h2 className="text-3xl font-black text-slate-900 tracking-tight leading-none mb-2">New Workspace</h2>
                            <p className="text-slate-400 font-medium">Orchestrate your local git repositories</p>
                        </div>
                    </div>
                    <button onClick={onClose} className="p-3 hover:bg-slate-100 rounded-full transition-all border border-transparent hover:border-slate-200 group">
                        <X size={24} className="text-slate-300 group-hover:text-slate-900" />
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-10 space-y-8">
                    {/* Workspace Name */}
                    <div className="space-y-2">
                        <label className="text-[10px] font-black text-slate-400 uppercase tracking-[2px] ml-1">
                            Environment Name
                        </label>
                        <input
                            autoFocus
                            type="text"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            placeholder="e.g. Project Alpha - Backend"
                            className="w-full px-6 py-5 bg-slate-50 border border-slate-100 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all text-slate-700 font-bold text-lg placeholder:text-slate-300 shadow-sm"
                            required
                        />
                    </div>

                    {/* Repository Selector */}
                    <div className="space-y-2 text-inter">
                        <label className="text-[10px] font-black text-slate-400 uppercase tracking-[2px] ml-1">
                            Local Repository (from /repos)
                        </label>
                        <div className="relative group">
                            {loadingRepos ? (
                                <div className="w-full px-6 py-5 bg-slate-50 border border-slate-100 rounded-2xl flex items-center gap-3">
                                    <Loader2 size={20} className="animate-spin text-slate-400" />
                                    <span className="text-slate-400 font-medium">Detecting repositories...</span>
                                </div>
                            ) : repoError ? (
                                <div className="w-full px-6 py-5 bg-rose-50 border border-rose-100 rounded-2xl flex items-center gap-3 text-rose-500">
                                    <AlertCircle size={20} />
                                    <span className="font-bold">Error loading repos. Check your /repos folder.</span>
                                </div>
                            ) : (
                                <select
                                    value={selectedRepo?.name || ''}
                                    onChange={(e) => {
                                        const repo = repos?.find(r => r.name === e.target.value) || null;
                                        setSelectedRepo(repo);
                                        setBranch(''); // Reset branch on repo change
                                    }}
                                    className="w-full px-6 py-5 bg-slate-50 border border-slate-100 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all text-slate-700 font-bold appearance-none cursor-pointer shadow-sm disabled:opacity-50"
                                    required
                                >
                                    <option value="" disabled>Select a repository...</option>
                                    {repos?.map((repo) => (
                                        <option key={repo.name} value={repo.name}>
                                            {repo.name}
                                        </option>
                                    ))}
                                </select>
                            )}
                            <div className="absolute right-6 top-1/2 -translate-y-1/2 pointer-events-none text-slate-400 group-focus-within:text-indigo-500">
                                <FolderCheck size={20} />
                            </div>
                        </div>
                    </div>

                    {/* Branch Selector */}
                    <div className={clsx(
                        "space-y-2 transition-all duration-300",
                        !selectedRepo ? "opacity-30 pointer-events-none grayscale" : "opacity-100"
                    )}>
                        <label className="text-[10px] font-black text-slate-400 uppercase tracking-[2px] ml-1">
                            Default Tracked Branch
                        </label>
                        <div className="relative group">
                            {loadingBranches ? (
                                <div className="w-full px-6 py-5 bg-slate-50 border border-slate-100 rounded-2xl flex items-center gap-3">
                                    <Loader2 size={20} className="animate-spin text-slate-400" />
                                    <span className="text-slate-400 font-medium">Fetching branches...</span>
                                </div>
                            ) : (
                                <select
                                    value={branch}
                                    onChange={(e) => setBranch(e.target.value)}
                                    className="w-full px-6 py-5 bg-slate-50 border border-slate-100 rounded-2xl focus:ring-4 focus:ring-indigo-500/10 focus:border-indigo-500 focus:bg-white outline-none transition-all text-slate-700 font-bold appearance-none cursor-pointer shadow-sm"
                                    required
                                    disabled={!branches || branches.length === 0}
                                >
                                    <option value="" disabled>Choose a branch...</option>
                                    {branches?.map((b) => (
                                        <option key={b} value={b}>{b}</option>
                                    ))}
                                </select>
                            )}
                            <div className="absolute right-6 top-1/2 -translate-y-1/2 pointer-events-none text-slate-400 group-focus-within:text-indigo-500">
                                <GitBranch size={20} />
                            </div>
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
                            disabled={mutation.isPending || !name || !selectedRepo || !branch}
                            className="flex-2 px-10 py-5 bg-indigo-600 text-white rounded-2xl font-black hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-2xl shadow-indigo-500/30 active:scale-95 flex items-center justify-center gap-4 text-lg"
                        >
                            {mutation.isPending ? <Loader2 size={24} className="animate-spin" /> : (
                                <>
                                    <span>Launch Workspace</span>
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

import { Plus } from 'lucide-react';
