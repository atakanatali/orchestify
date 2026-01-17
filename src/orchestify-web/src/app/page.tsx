'use client';

import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/lib/api';
import { CheckCircle2, Loader2, FolderKanban, ListTodo, Zap, Activity, ChevronRight } from 'lucide-react';
import clsx from 'clsx';

export default function DashboardPage() {
  const { data: stats, isLoading } = useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: dashboardApi.stats,
  });

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-full">
        <Loader2 className="animate-spin text-blue-600" size={32} />
      </div>
    );
  }

  return (
    <div className="p-12 max-w-6xl mx-auto">
      <div className="mb-12">
        <h1 className="text-4xl font-black text-slate-900 tracking-tight mb-2">Welcome to Orchestify</h1>
        <p className="text-slate-500 text-lg font-medium opacity-80">AI-driven code orchestration</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8 mb-12">
        <StatCard
          title="Workspaces"
          value={stats?.totalWorkspaces ?? 0}
          icon={<FolderKanban className="text-teal-600" size={24} />}
          gradient="from-teal-50 to-white"
        />
        <StatCard
          title="Total Tasks"
          value={stats?.totalTasks ?? 0}
          icon={<ListTodo className="text-[var(--brand-orange)]" size={24} />}
          gradient="from-orange-50 to-white"
        />
        <StatCard
          title="Completed"
          value={stats?.completedTasks ?? 0}
          icon={<CheckCircle2 className="text-emerald-500" size={24} />}
          gradient="from-emerald-50 to-white"
        />
        <StatCard
          title="Health Score"
          value={98}
          unit="%"
          icon={<Activity className="text-blue-500" size={24} />}
          gradient="from-blue-50 to-white"
        />
      </div>

      <div className="bg-white rounded-3xl p-10 border border-slate-100 shadow-[0_4px_20px_rgba(0,0,0,0.03)] transition-all hover:shadow-[0_8px_30px_rgba(0,0,0,0.05)]">
        <div className="flex items-center justify-between mb-10">
          <div className="flex flex-col gap-1">
            <h2 className="text-2xl font-black text-slate-900 flex items-center gap-3">
              <div className="p-2 bg-amber-100 rounded-xl text-amber-500">
                <Zap size={24} className="fill-amber-500" />
              </div>
              Active Executions
            </h2>
            <p className="text-sm text-slate-400 font-medium">Real-time status of your orchestration pipelines</p>
          </div>
          <button className="px-6 py-2.5 bg-slate-50 text-slate-600 text-xs font-black uppercase tracking-[2px] rounded-xl hover:bg-slate-100 transition-all border border-slate-100 active:scale-95">
            View History
          </button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <QueueCard
            label="Queued"
            value={stats?.queuedAttempts ?? 0}
            color="bg-slate-100 text-slate-500"
            description="Awaiting worker assignment"
          />
          <QueueCard
            label="Running"
            value={stats?.runningAttempts ?? 0}
            color="bg-amber-100 text-amber-600 border border-amber-200"
            description="Processing steps now"
            isActive
          />
          <QueueCard
            label="Failed"
            value={stats?.failedAttempts ?? 0}
            color="bg-rose-100 text-rose-600"
            description="Terminated with errors"
          />
        </div>
      </div>
    </div>
  );
}

function StatCard({ title, value, unit = "", icon, gradient }: { title: string; value: number; unit?: string; icon: React.ReactNode; gradient: string }) {
  return (
    <div className={clsx("rounded-[32px] p-8 border border-slate-100 shadow-sm hover:shadow-xl transition-all group bg-gradient-to-br bg-white", gradient)}>
      <div className="flex items-center justify-between mb-6">
        <div className="p-4 rounded-2xl bg-white shadow-md border border-slate-50 group-hover:scale-110 transition-transform duration-300">
          {icon}
        </div>
      </div>
      <div className="flex items-baseline gap-1">
        <div className="text-5xl font-black text-slate-900 tracking-tighter">{value}</div>
        <div className="text-2xl font-black text-slate-300 tracking-tight">{unit}</div>
      </div>
      <div className="text-[10px] font-black text-slate-400 uppercase tracking-[2px] mt-2">{title}</div>
    </div>
  );
}

function QueueCard({ label, value, color, description, isActive = false }: { label: string; value: number; color: string; description: string; isActive?: boolean }) {
  return (
    <div className={clsx(
      "flex flex-col gap-4 p-8 rounded-3xl border transition-all duration-300 relative group/queue overflow-hidden",
      isActive
        ? "bg-amber-50/50 border-amber-200 shadow-lg shadow-amber-500/5"
        : "bg-white border-slate-50 hover:border-slate-200 hover:shadow-md"
    )}>
      <div className={clsx("w-14 h-14 rounded-2xl flex items-center justify-center font-black text-2xl shadow-sm", color)}>
        {value}
      </div>
      <div>
        <div className="font-black text-slate-900 text-lg leading-none mb-2">{label}</div>
        <div className="text-xs text-slate-400 font-medium leading-relaxed">{description}</div>
      </div>
      <div className="absolute top-6 right-6 p-2 rounded-full bg-slate-50 text-slate-300 opacity-0 group-hover/queue:opacity-100 transition-all group-hover/queue:translate-x-0 translate-x-4">
        <ChevronRight size={16} />
      </div>
      {isActive && (
        <div className="absolute bottom-0 left-0 right-0 h-1 bg-gradient-to-r from-transparent via-amber-400 to-transparent animate-pulse" />
      )}
    </div>
  );
}
