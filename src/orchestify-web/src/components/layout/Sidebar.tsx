'use client';

import Image from 'next/image';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { workspacesApi, Workspace } from '@/lib/api';
import { LayoutDashboard, FolderKanban, Plus, Settings, ChevronRight, Trash2 } from 'lucide-react';
import clsx from 'clsx';
import { CreateWorkspaceModal } from '@/components/modals/CreateWorkspaceModal';

export function Sidebar() {
    const pathname = usePathname();
    const [showCreateWorkspace, setShowCreateWorkspace] = useState(false);
    const { data: workspaces } = useQuery({
        queryKey: ['workspaces'],
        queryFn: workspacesApi.list,
    });

    return (
        <aside className="w-64 bg-white border-r border-[var(--border)] flex flex-col">
            {/* Logo */}
            <div className="p-4 border-b border-[var(--border)]">
                <Link href="/" className="flex items-center gap-3">
                    <Image src="/logo.png" alt="Orchestify" width={40} height={40} className="rounded-lg" />
                    <span className="font-bold text-lg text-[var(--text-primary)]">Orchestify</span>
                </Link>
            </div>

            {/* Navigation */}
            <nav className="flex-1 overflow-y-auto p-3">
                <NavItem href="/" icon={<LayoutDashboard size={20} />} label="Dashboard" active={pathname === '/'} />

                <div className="mt-6">
                    <div className="flex items-center justify-between px-3 mb-2">
                        <span className="text-xs font-semibold text-[var(--text-secondary)] uppercase tracking-wider">
                            Workspaces
                        </span>
                        <button
                            onClick={() => setShowCreateWorkspace(true)}
                            className="p-1 hover:bg-gray-100 rounded transition-colors"
                        >
                            <Plus size={16} className="text-[var(--text-secondary)]" />
                        </button>
                    </div>

                    <div className="space-y-1">
                        {workspaces?.items?.map((workspace) => (
                            <WorkspaceItem
                                key={workspace.id}
                                workspace={workspace}
                                isActive={pathname.includes(workspace.id)}
                            />
                        ))}
                    </div>
                </div>
            </nav>

            {/* Modals */}
            {showCreateWorkspace && (
                <CreateWorkspaceModal onClose={() => setShowCreateWorkspace(false)} />
            )}

            {/* Footer */}
            <div className="p-3 border-t border-[var(--border)]">
                <NavItem href="/settings" icon={<Settings size={20} />} label="Settings" active={pathname === '/settings'} />
            </div>
        </aside>
    );
}

function NavItem({ href, icon, label, active }: { href: string; icon: React.ReactNode; label: string; active: boolean }) {
    return (
        <Link
            href={href}
            className={clsx(
                'flex items-center gap-3 px-3 py-2.5 rounded-lg transition-all',
                active
                    ? 'bg-[var(--primary)] bg-opacity-10 text-[var(--primary)]'
                    : 'text-[var(--text-secondary)] hover:bg-gray-100'
            )}
        >
            {icon}
            <span className="font-medium">{label}</span>
        </Link>
    );
}

function WorkspaceItem({ workspace, isActive }: { workspace: Workspace; isActive: boolean }) {
    const queryClient = useQueryClient();
    const deleteMutation = useMutation({
        mutationFn: () => workspacesApi.delete(workspace.id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['workspaces'] });
        }
    });

    const handleDelete = (e: React.MouseEvent) => {
        e.preventDefault();
        if (confirm(`Are you sure you want to delete workspace "${workspace.name}"?`)) {
            deleteMutation.mutate();
        }
    };

    return (
        <Link
            href={`/workspaces/${workspace.id}`}
            className={clsx(
                'flex items-center gap-3 px-3 py-2 rounded-lg transition-all group relative',
                isActive
                    ? 'bg-[var(--primary)] bg-opacity-10 text-[var(--primary)]'
                    : 'text-[var(--text-secondary)] hover:bg-gray-100'
            )}
        >
            <FolderKanban size={18} />
            <span className="flex-1 truncate font-medium">{workspace.name}</span>
            <div className="flex items-center gap-1">
                <button
                    onClick={handleDelete}
                    className="p-1 hover:bg-red-100 text-red-500 rounded opacity-0 group-hover:opacity-100 transition-opacity"
                >
                    <Trash2 size={14} />
                </button>
                <ChevronRight size={16} className="opacity-0 group-hover:opacity-100 transition-opacity" />
            </div>
        </Link>
    );
}
