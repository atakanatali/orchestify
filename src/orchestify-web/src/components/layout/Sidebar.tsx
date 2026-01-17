'use client';

import Image from 'next/image';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { workspacesApi, Workspace } from '@/lib/api';
import { LayoutDashboard, Plus, Settings, ChevronRight, ChevronLeft, Trash2, Zap, FolderKanban } from 'lucide-react';
import clsx from 'clsx';
import { CreateWorkspaceModal } from '@/components/modals/CreateWorkspaceModal';

export function Sidebar() {
    const pathname = usePathname();
    const [isCollapsed, setIsCollapsed] = useState(false);
    const [showCreateWorkspace, setShowCreateWorkspace] = useState(false);
    const { data: workspaces } = useQuery({
        queryKey: ['workspaces'],
        queryFn: workspacesApi.list,
    });

    return (
        <div className="flex h-full relative group/sidebar">
            {/* Main Nav Pane */}
            <aside
                className={clsx(
                    "flex flex-col bg-[var(--bg-sidebar-nav)] border-r border-[var(--border-light)] transition-all duration-300 ease-in-out relative z-30",
                    isCollapsed ? "w-16" : "w-64"
                )}
            >
                {/* Header with Logo */}
                <div className={clsx(
                    "p-4 mb-2 flex items-center transition-all duration-300",
                    isCollapsed ? "justify-center" : "justify-between"
                )}>
                    <Link href="/" className="flex items-center gap-3 group/logo">
                        <div className="shrink-0 transition-transform active:scale-90">
                            <Image
                                src="/logo_v2.png"
                                alt="Orchestify"
                                width={32}
                                height={32}
                                className="rounded-lg shadow-md border border-white/10"
                            />
                        </div>
                        {!isCollapsed && (
                            <span className="text-xl font-black text-slate-800 tracking-tight animate-in fade-in slide-in-from-left-2">
                                Orchestify
                            </span>
                        )}
                    </Link>
                </div>

                {/* Collapse Toggle - Floating Button */}
                <button
                    onClick={() => setIsCollapsed(!isCollapsed)}
                    className={clsx(
                        "absolute -right-3 top-12 w-6 h-6 bg-white border border-slate-200 rounded-full flex items-center justify-center text-slate-400 hover:text-blue-600 hover:border-blue-200 transition-all shadow-sm z-40 active:scale-95",
                        isCollapsed && "rotate-180"
                    )}
                >
                    <ChevronLeft size={14} />
                </button>

                {/* Main Navigation */}
                <nav className="flex-1 overflow-y-auto px-3 py-2 space-y-6 scrollbar-none">
                    <div className="space-y-1">
                        <NavItem
                            href="/"
                            icon={<LayoutDashboard size={20} />}
                            label="Dashboard"
                            active={pathname === '/'}
                            isCollapsed={isCollapsed}
                        />
                        <NavItem
                            href="/flows"
                            icon={<Zap size={20} />}
                            label="Flows"
                            active={pathname.includes('/flows')}
                            isCollapsed={isCollapsed}
                        />
                    </div>

                    {/* Workspaces Section */}
                    <div>
                        <div className={clsx(
                            "mb-2 flex items-center justify-between px-3 transition-opacity",
                            isCollapsed ? "opacity-0" : "opacity-100"
                        )}>
                            <span className="text-[10px] font-black text-slate-400 uppercase tracking-[2px]">Workspaces</span>
                            <button
                                onClick={() => setShowCreateWorkspace(true)}
                                className="p-1 hover:bg-slate-200 rounded text-slate-500 transition-all active:scale-90"
                            >
                                <Plus size={14} />
                            </button>
                        </div>

                        <div className="space-y-1">
                            {workspaces?.items?.map((workspace) => (
                                <WorkspaceItem
                                    key={workspace.id}
                                    workspace={workspace}
                                    isActive={pathname.includes(workspace.id)}
                                    isCollapsed={isCollapsed}
                                />
                            ))}
                        </div>
                    </div>
                </nav>
            </aside>

            {/* Modals */}
            {showCreateWorkspace && (
                <CreateWorkspaceModal onClose={() => setShowCreateWorkspace(false)} />
            )}
        </div>
    );
}

function NavItem({
    href,
    icon,
    label,
    active,
    isCollapsed
}: {
    href: string;
    icon: React.ReactNode;
    label: string;
    active: boolean;
    isCollapsed: boolean;
}) {
    return (
        <Link
            href={href}
            title={isCollapsed ? label : ""}
            className={clsx(
                'flex items-center gap-3 px-3 py-2.5 rounded-xl transition-all relative group/nav',
                isCollapsed ? 'justify-center' : '',
                active
                    ? 'bg-blue-600/10 text-blue-600'
                    : 'text-slate-600 hover:bg-slate-100 active:bg-slate-200'
            )}
        >
            <div className="shrink-0 transition-transform duration-300">
                {icon}
            </div>
            {!isCollapsed && (
                <span className="font-bold text-[13px] tracking-tight">{label}</span>
            )}
            {active && !isCollapsed && (
                <div className="absolute left-0 top-2 bottom-2 w-1 bg-blue-600 rounded-r-full" />
            )}
        </Link>
    );
}

function WorkspaceItem({
    workspace,
    isActive,
    isCollapsed
}: {
    workspace: Workspace;
    isActive: boolean;
    isCollapsed: boolean;
}) {
    const queryClient = useQueryClient();
    const deleteMutation = useMutation({
        mutationFn: () => workspacesApi.delete(workspace.id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['workspaces'] });
        }
    });

    const handleDelete = (e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        if (confirm(`Delete workspace "${workspace.name}"?`)) {
            deleteMutation.mutate();
        }
    };

    return (
        <Link
            href={`/workspaces/${workspace.id}`}
            title={isCollapsed ? workspace.name : ""}
            className={clsx(
                'flex items-center gap-3 px-3 py-2 rounded-xl transition-all group relative',
                isCollapsed ? 'justify-center' : '',
                isActive
                    ? 'bg-blue-600 text-white shadow-lg shadow-blue-500/20'
                    : 'text-slate-600 hover:bg-slate-100 active:bg-slate-200'
            )}
        >
            <div className={clsx(
                "w-7 h-7 rounded-lg flex items-center justify-center text-[11px] font-black shrink-0 transition-all shadow-sm",
                isActive ? "bg-white/20 text-white" : "bg-indigo-500 text-white"
            )}>
                {workspace.name.substring(0, 1).toUpperCase()}
            </div>

            {!isCollapsed && (
                <span className="flex-1 truncate text-[13px] font-bold tracking-tight">{workspace.name}</span>
            )}

            {!isCollapsed && (
                <button
                    onClick={handleDelete}
                    className={clsx(
                        "p-1 hover:bg-red-500 hover:text-white rounded-lg transition-all shrink-0 opacity-0 group-hover:opacity-100",
                        isActive ? "text-white/50" : "text-slate-300"
                    )}
                >
                    <Trash2 size={12} />
                </button>
            )}

            {isActive && !isCollapsed && (
                <div className="absolute left-0 top-2 bottom-2 w-1 bg-white rounded-r-full" />
            )}
        </Link>
    );
}
