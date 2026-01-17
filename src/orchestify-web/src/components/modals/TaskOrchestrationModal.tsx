'use client';

import { useState, useRef, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { messagesApi, tasksApi, TaskMessage, Task } from '@/lib/api';
import { X, Maximize2, Minimize2, Terminal, Loader2, Send, Bot, User, Sparkles, Circle, ChevronRight } from 'lucide-react';
import clsx from 'clsx';

interface TaskOrchestrationModalProps {
    task: Task;
    boardId: string;
    onClose: () => void;
}

export function TaskOrchestrationModal({ task, boardId, onClose }: TaskOrchestrationModalProps) {
    const [input, setInput] = useState('');
    const [isMaximized, setIsMaximized] = useState(false);
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const queryClient = useQueryClient();

    const isRunning = task.status === 'In Progress';

    const { data: messages, isLoading } = useQuery({
        queryKey: ['task-messages', task.id],
        queryFn: () => messagesApi.list(task.id),
        refetchInterval: isRunning ? 1000 : 3000,
    });

    const sendMutation = useMutation({
        mutationFn: (content: string) => messagesApi.send(task.id, content),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['task-messages', task.id] });
            setInput('');
        },
    });

    const runMutation = useMutation({
        mutationFn: () => tasksApi.run(boardId, task.id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['tasks', boardId] });
        },
    });

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const handleSend = () => {
        if (!input.trim() || sendMutation.isPending || isRunning) return;
        sendMutation.mutate(input);
    };

    const handleStartTask = () => {
        // If task has a description, use it as first message
        if (task.description && (!messages || messages.length === 0)) {
            sendMutation.mutate(task.description);
        }
        runMutation.mutate();
    };

    return (
        <div className={clsx(
            "fixed transition-all duration-500 ease-in-out z-[300] shadow-[0_30px_100px_rgba(0,0,0,0.5)] border border-slate-700/50 backdrop-blur-xl overflow-hidden flex flex-col",
            isMaximized
                ? "inset-4 rounded-3xl"
                : "bottom-8 right-8 w-[500px] h-[700px] rounded-2xl"
        )}>
            {/* Header */}
            <div className="bg-slate-900/90 p-5 flex items-center justify-between border-b border-slate-800 shrink-0">
                <div className="flex items-center gap-4">
                    <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center text-white shadow-lg shadow-indigo-500/30">
                        <Sparkles size={20} />
                    </div>
                    <div>
                        <h3 className="text-white font-black text-sm tracking-tight leading-none mb-1 truncate max-w-[200px]">{task.title}</h3>
                        <div className="flex items-center gap-2">
                            <span className={clsx(
                                "text-[10px] font-black uppercase tracking-widest flex items-center gap-1.5",
                                isRunning ? "text-amber-400" : "text-emerald-400"
                            )}>
                                <div className={clsx("w-1.5 h-1.5 rounded-full", isRunning ? "bg-amber-400 animate-pulse" : "bg-emerald-400")} />
                                {isRunning ? 'Processing' : 'Ready'}
                            </span>
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

            {/* Task Context */}
            {task.description && (
                <div className="px-6 py-4 bg-slate-800/50 border-b border-slate-700/30 shrink-0">
                    <div className="flex items-center gap-2 text-[10px] font-black text-slate-500 uppercase tracking-[2px] mb-2">
                        <Terminal size={12} />
                        Task Logic
                    </div>
                    <p className="text-slate-400 text-xs leading-relaxed line-clamp-3">{task.description}</p>
                </div>
            )}

            {/* Messages Area */}
            <div className="flex-1 overflow-y-auto p-6 space-y-4 bg-gradient-to-b from-slate-900 to-slate-950">
                {isLoading ? (
                    <div className="flex items-center justify-center h-full">
                        <Loader2 className="animate-spin text-indigo-400" size={24} />
                    </div>
                ) : messages?.length === 0 ? (
                    <div className="flex flex-col items-center justify-center h-full text-center px-8">
                        <div className="w-16 h-16 rounded-3xl bg-indigo-500/10 flex items-center justify-center mb-6">
                            <Bot size={32} className="text-indigo-400" />
                        </div>
                        <p className="text-slate-400 font-medium text-sm leading-relaxed mb-6">
                            Ready to orchestrate <span className="text-indigo-400 font-bold">{task.title}</span>.
                            <br />Click Start to begin execution.
                        </p>
                        <button
                            onClick={handleStartTask}
                            disabled={runMutation.isPending}
                            className="px-6 py-3 bg-indigo-600 text-white rounded-xl font-black hover:bg-indigo-700 transition-all shadow-lg shadow-indigo-500/30 active:scale-95 flex items-center gap-2"
                        >
                            {runMutation.isPending ? <Loader2 size={18} className="animate-spin" /> : <ChevronRight size={18} />}
                            Start Task
                        </button>
                    </div>
                ) : (
                    messages?.map((msg) => (
                        <MessageBubble key={msg.id} message={msg} />
                    ))
                )}
                <div ref={messagesEndRef} />
            </div>

            {/* Input Area */}
            <div className="p-4 bg-slate-900 border-t border-slate-700/50 shrink-0">
                {isRunning ? (
                    <div className="flex items-center justify-center gap-3 py-3 text-amber-400">
                        <Loader2 size={18} className="animate-spin" />
                        <span className="text-sm font-bold">Agent is processing...</span>
                    </div>
                ) : (
                    <div className="flex items-center gap-3 bg-slate-800 rounded-2xl p-2 pl-5 border border-slate-700/50 focus-within:border-indigo-500 focus-within:ring-4 focus-within:ring-indigo-500/10 transition-all">
                        <input
                            type="text"
                            value={input}
                            onChange={(e) => setInput(e.target.value)}
                            onKeyDown={(e) => e.key === 'Enter' && handleSend()}
                            placeholder="Ask Antigravity something..."
                            className="flex-1 bg-transparent text-white placeholder:text-slate-500 outline-none text-sm font-medium"
                        />
                        <button
                            onClick={handleSend}
                            disabled={sendMutation.isPending || !input.trim()}
                            className="p-3 bg-indigo-600 text-white rounded-xl hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-lg shadow-indigo-500/30 active:scale-90"
                        >
                            {sendMutation.isPending ? <Loader2 size={18} className="animate-spin" /> : <Send size={18} />}
                        </button>
                    </div>
                )}
            </div>

            {/* Footer Status */}
            <div className="px-4 py-3 bg-slate-900/80 border-t border-slate-800 flex items-center justify-between shrink-0">
                <div className="text-[10px] font-black text-slate-500 uppercase tracking-widest flex items-center gap-2">
                    <ChevronRight size={12} className="text-blue-500" />
                    {isRunning ? 'Executing orchestration' : 'System ready for input'}
                </div>
                <div className="flex items-center gap-1.5">
                    <Circle size={8} className={clsx("fill-current", isRunning ? "text-amber-500" : "text-emerald-500")} />
                    <span className="text-[10px] font-black text-slate-400 uppercase tracking-widest">
                        {isRunning ? 'Active' : 'Connected'}
                    </span>
                </div>
            </div>
        </div>
    );
}

function MessageBubble({ message }: { message: TaskMessage }) {
    const isAgent = message.sender === 'Agent';

    return (
        <div className={clsx("flex gap-3", isAgent ? "justify-start" : "justify-end")}>
            {isAgent && (
                <div className="w-8 h-8 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center shrink-0 shadow-md">
                    <Bot size={16} className="text-white" />
                </div>
            )}
            <div className={clsx(
                "max-w-[80%] rounded-2xl px-5 py-4 shadow-lg",
                isAgent ? "bg-slate-800 text-slate-100 rounded-tl-none" : "bg-indigo-600 text-white rounded-tr-none"
            )}>
                <p className="text-sm font-medium leading-relaxed whitespace-pre-wrap">{message.content}</p>
            </div>
            {!isAgent && (
                <div className="w-8 h-8 rounded-xl bg-slate-700 flex items-center justify-center shrink-0 shadow-md">
                    <User size={16} className="text-slate-300" />
                </div>
            )}
        </div>
    );
}
