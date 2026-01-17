'use client';

import { useState, useRef, useEffect } from 'react';
import { useLogs } from '@/hooks/use-logs';
import { X, Maximize2, Minimize2, Terminal, Loader2, Circle, ChevronRight } from 'lucide-react';
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
    const [isMaximized, setIsMaximized] = useState(false);
    const scrollRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (scrollRef.current) {
            scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
        }
    }, [logs]);

    return (
        <div className={clsx(
            "fixed transition-all duration-500 ease-in-out z-[300] shadow-[0_30px_100px_rgba(0,0,0,0.5)] border border-slate-700/50 backdrop-blur-xl overflow-hidden",
            isMaximized
                ? "inset-4 rounded-3xl"
                : "bottom-8 right-8 w-[500px] h-[600px] rounded-2xl"
        )}>
            {/* Header */}
            <div className="bg-slate-900/90 p-5 flex items-center justify-between border-b border-slate-800">
                <div className="flex items-center gap-4">
                    <div className="w-10 h-10 rounded-xl bg-blue-600 flex items-center justify-center text-white shadow-lg shadow-blue-500/20">
                        <Terminal size={20} />
                    </div>
                    <div>
                        <h3 className="text-white font-black text-sm tracking-tight leading-none mb-1">{taskTitle}</h3>
                        <div className="flex items-center gap-2">
                            <span className={clsx(
                                "text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5",
                                status === 'connected' ? "text-emerald-400" : "text-amber-400"
                            )}>
                                <div className={clsx("w-1.5 h-1.5 rounded-full", status === 'connected' ? "bg-emerald-400 animate-pulse" : "bg-amber-400")} />
                                {status}
                            </span>
                            <span className="text-[10px] text-slate-500 font-mono">ID: {attemptId.split('-')[0]}</span>
                        </div>
                    </div>
                </div>

                <div className="flex items-center gap-1">
                    <button
                        onClick={() => setIsMaximized(!isMaximized)}
                        className="p-2 hover:bg-slate-800 rounded-lg text-slate-400 hover:text-white transition-all"
                    >
                        {isMaximized ? <Minimize2 size={18} /> : <Maximize2 size={18} />}
                    </button>
                    <button
                        onClick={onClose}
                        className="p-2 hover:bg-rose-500 rounded-lg text-slate-400 hover:text-white transition-all shadow-lg"
                    >
                        <X size={18} />
                    </button>
                </div>
            </div>

            {/* Log Area */}
            <div
                ref={scrollRef}
                className="bg-slate-950 p-6 h-[calc(100%-80px)] overflow-y-auto font-mono text-[13px] leading-relaxed scroll-smooth"
            >
                {logs.length === 0 && (
                    <div className="flex flex-col items-center justify-center h-full text-slate-600 gap-4">
                        <Loader2 className="animate-spin" size={32} />
                        <p className="font-bold tracking-widest uppercase text-xs">Waiting for stream...</p>
                    </div>
                )}
                <div className="space-y-1">
                    {logs.map((log, i) => (
                        <div key={log.id} className="flex gap-4 group">
                            <span className="text-slate-700 shrink-0 select-none opacity-50 group-hover:opacity-100">{i + 1}</span>
                            <span className={clsx(
                                "break-all",
                                log.type === 'error' ? "text-rose-400" :
                                    log.type === 'warn' ? "text-amber-300" :
                                        log.type === 'info' ? "text-blue-300" : "text-slate-300"
                            )}>
                                {log.message}
                            </span>
                        </div>
                    ))}
                    {status === 'connected' && (
                        <div className="flex gap-4 mt-2">
                            <span className="text-slate-700 shrink-0 select-none">{logs.length + 1}</span>
                            <div className="w-2 h-4 bg-blue-500/50 animate-pulse rounded-sm" />
                        </div>
                    )}
                </div>
            </div>

            {/* Command Input Simulated / Footer */}
            <div className="absolute bottom-0 left-0 right-0 p-4 bg-slate-900 border-t border-slate-800 flex items-center justify-between">
                <div className="text-[10px] font-black text-slate-500 uppercase tracking-widest flex items-center gap-2">
                    <ChevronRight size={12} className="text-blue-500" />
                    System ready for input
                </div>
                <div className="flex items-center gap-3">
                    <div className="flex items-center gap-1.5">
                        <Circle size={8} className="fill-emerald-500 text-emerald-500 shadow-lg shadow-emerald-500/50" />
                        <span className="text-[10px] font-black text-slate-400 uppercase tracking-widest">Connected</span>
                    </div>
                </div>
            </div>
        </div>
    );
}
