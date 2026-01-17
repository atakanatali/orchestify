'use client';

import { useRef, useEffect } from 'react';
import { X, Terminal as TerminalIcon, Loader2, Maximize2, Minimize2 } from 'lucide-react';
import { useLogs } from '@/hooks/use-logs';
import { useState } from 'react';
import clsx from 'clsx';

export function LiveLogs({
    attemptId,
    taskTitle,
    onClose
}: {
    attemptId: string;
    taskTitle: string;
    onClose: () => void
}) {
    const { logs, status } = useLogs(attemptId);
    const scrollRef = useRef<HTMLDivElement>(null);
    const [isMaximized, setIsMaximized] = useState(false);

    useEffect(() => {
        if (scrollRef.current) {
            scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
        }
    }, [logs]);

    return (
        <div className={clsx(
            "fixed bottom-4 right-4 z-[150] flex flex-col bg-[#0F172A] border border-slate-700 shadow-2xl rounded-xl transition-all duration-300 overflow-hidden",
            isMaximized ? "inset-4 shadow-none" : "w-[500px] h-[400px]"
        )}>
            {/* Header */}
            <div className="flex items-center justify-between px-4 py-3 bg-slate-800 border-b border-slate-700 select-none">
                <div className="flex items-center gap-2 overflow-hidden">
                    <TerminalIcon size={16} className="text-emerald-400 shrink-0" />
                    <span className="text-xs font-mono text-slate-300 truncate">
                        {taskTitle} - {attemptId.split('-')[0]}
                    </span>
                    {status === 'connected' && <div className="w-1.5 h-1.5 rounded-full bg-emerald-500 animate-pulse shrink-0" />}
                </div>

                <div className="flex items-center gap-1 shrink-0">
                    <button
                        onClick={() => setIsMaximized(!isMaximized)}
                        className="p-1.5 hover:bg-slate-700 rounded text-slate-400 transition-colors"
                    >
                        {isMaximized ? <Minimize2 size={14} /> : <Maximize2 size={14} />}
                    </button>
                    <button
                        onClick={onClose}
                        className="p-1.5 hover:bg-red-500/20 hover:text-red-400 rounded text-slate-400 transition-colors"
                    >
                        <X size={14} />
                    </button>
                </div>
            </div>

            {/* Logs Area */}
            <div
                ref={scrollRef}
                className="flex-1 p-4 font-mono text-sm overflow-y-auto bg-black/40"
            >
                {logs.length === 0 && status === 'connecting' && (
                    <div className="flex items-center gap-2 text-slate-500 italic">
                        <Loader2 size={14} className="animate-spin" />
                        Initializing stream...
                    </div>
                )}

                {logs.map((log) => (
                    <div key={log.id} className="mb-1 flex gap-3 group">
                        <span className="text-slate-600 shrink-0 select-none opacity-50 group-hover:opacity-100">
                            [{log.timestamp}]
                        </span>
                        <span className={clsx(
                            "break-all",
                            log.type === 'error' ? 'text-rose-400' :
                                log.type === 'warn' ? 'text-amber-400' :
                                    log.type === 'success' ? 'text-emerald-400' :
                                        'text-slate-300'
                        )}>
                            {log.message}
                        </span>
                    </div>
                ))}

                {status === 'error' && (
                    <div className="mt-2 text-rose-400 flex items-center gap-2">
                        <X size={14} /> Stream connection lost.
                    </div>
                )}
            </div>

            {/* Footer Info */}
            <div className="px-4 py-2 bg-slate-800/50 border-t border-slate-700 text-[10px] uppercase tracking-wider text-slate-500 font-mono flex justify-between">
                <span>Channel: sse://logs</span>
                <span>Lines: {logs.length}</span>
            </div>
        </div>
    );
}
