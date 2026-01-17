'use client';

import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/lib/api';
import { LayoutGrid, CheckCircle2, Loader2, AlertCircle, FolderKanban, ListTodo } from 'lucide-react';

export default function DashboardPage() {
  const { data: stats, isLoading } = useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: dashboardApi.stats,
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
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-[var(--text-primary)]">Dashboard</h1>
        <p className="text-[var(--text-secondary)]">Overview of your workspace activity</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <StatCard
          title="Workspaces"
          value={stats?.totalWorkspaces ?? 0}
          icon={<FolderKanban className="text-[var(--primary)]" />}
          color="primary"
        />
        <StatCard
          title="Total Tasks"
          value={stats?.totalTasks ?? 0}
          icon={<ListTodo className="text-[var(--secondary)]" />}
          color="secondary"
        />
        <StatCard
          title="Completed"
          value={stats?.completedTasks ?? 0}
          icon={<CheckCircle2 className="text-[var(--success)]" />}
          color="success"
        />
        <StatCard
          title="In Progress"
          value={stats?.inProgressTasks ?? 0}
          icon={<Loader2 className="text-[var(--warning)]" />}
          color="warning"
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white rounded-xl p-6 border border-[var(--border)] shadow-sm">
          <h2 className="text-lg font-semibold mb-4">Execution Queue</h2>
          <div className="space-y-3">
            <QueueRow label="Queued" value={stats?.queuedAttempts ?? 0} color="bg-gray-400" />
            <QueueRow label="Running" value={stats?.runningAttempts ?? 0} color="bg-[var(--warning)]" />
            <QueueRow label="Failed" value={stats?.failedAttempts ?? 0} color="bg-[var(--error)]" />
          </div>
        </div>

        <div className="bg-white rounded-xl p-6 border border-[var(--border)] shadow-sm">
          <h2 className="text-lg font-semibold mb-4">Quick Actions</h2>
          <div className="grid grid-cols-2 gap-3">
            <ActionButton label="New Workspace" />
            <ActionButton label="New Board" />
            <ActionButton label="View Logs" />
            <ActionButton label="Settings" />
          </div>
        </div>
      </div>
    </div>
  );
}

function StatCard({ title, value, icon, color }: { title: string; value: number; icon: React.ReactNode; color: string }) {
  return (
    <div className="bg-white rounded-xl p-6 border border-[var(--border)] shadow-sm hover:shadow-md transition-shadow">
      <div className="flex items-center justify-between mb-3">
        <div className="p-2 rounded-lg bg-gray-50">{icon}</div>
      </div>
      <div className="text-3xl font-bold text-[var(--text-primary)]">{value}</div>
      <div className="text-sm text-[var(--text-secondary)]">{title}</div>
    </div>
  );
}

function QueueRow({ label, value, color }: { label: string; value: number; color: string }) {
  return (
    <div className="flex items-center justify-between">
      <div className="flex items-center gap-2">
        <div className={`w-2 h-2 rounded-full ${color}`} />
        <span className="text-sm text-[var(--text-secondary)]">{label}</span>
      </div>
      <span className="font-semibold">{value}</span>
    </div>
  );
}

function ActionButton({ label }: { label: string }) {
  return (
    <button className="px-4 py-3 border border-[var(--border)] rounded-lg text-sm font-medium text-[var(--text-secondary)] hover:bg-gray-50 hover:border-[var(--primary)] hover:text-[var(--primary)] transition-all">
      {label}
    </button>
  );
}
