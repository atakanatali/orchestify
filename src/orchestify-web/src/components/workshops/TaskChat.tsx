'use client';

import { useState, useRef, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { messagesApi, TaskMessage, AgentToolCall } from '@/lib/api';
import { Send, Loader2, Bot, User, Terminal, Sparkles, FileText } from 'lucide-react';
import clsx from 'clsx';

interface TaskChatProps {
    taskId: string;
    taskTitle: string;
    taskDescription?: string;
}

export function TaskChat({ taskId, taskTitle, taskDescription }: TaskChatProps) {
    const [input, setInput] = useState('');
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const queryClient = useQueryClient();

    const { data: messages, isLoading } = useQuery({
        queryKey: ['task-messages', taskId],
        queryFn: () => messagesApi.list(taskId),
        refetchInterval: 3000,
    });

    const sendMutation = useMutation({
        mutationFn: (content: string) => messagesApi.send(taskId, content),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['task-messages', taskId] });
            setInput('');
        },
    });

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const handleSend = () => {
        if (!input.trim() || sendMutation.isPending) return;
        sendMutation.mutate(input);
    };

    const parseToolCall = (action: string | null): AgentToolCall | null => {
        if (!action) return null;
        try {
            return JSON.parse(action) as AgentToolCall;
        } catch {
            return null;
        }
    };

    return (
        <div className="flex flex-col h-full bg-slate-900 overflow-hidden">
            {/* Header */}
            <div className="px-6 py-5 bg-gradient-to-r from-slate-800 to-slate-900 border-b border-slate-700/50 flex items-center gap-4 shrink-0">
                <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center shadow-lg shadow-indigo-500/30">
                    <Sparkles size={20} className="text-white" />
                </div>
                <div>
                    <h3 className="text-white font-black text-sm tracking-tight">Antigravity</h3>
                    <p className="text-slate-400 text-[10px] font-bold uppercase tracking-[2px]">AI Agent • Online</p>
                </div>
            </div>

            {/* Task Context - Shows description */}
            {taskDescription && (
                <div className="px-6 py-4 bg-slate-800/50 border-b border-slate-700/30 shrink-0">
                    <div className="flex items-center gap-2 text-[10px] font-black text-slate-500 uppercase tracking-[2px] mb-2">
                        <FileText size={12} />
                        Task Context
                    </div>
                    <p className="text-slate-400 text-xs leading-relaxed line-clamp-3">{taskDescription}</p>
                </div>
            )}

            {/* Messages */}
            <div className="flex-1 overflow-y-auto p-6 space-y-6 bg-gradient-to-b from-slate-900 to-slate-950">
                {isLoading ? (
                    <div className="flex items-center justify-center h-full">
                        <Loader2 className="animate-spin text-indigo-400" size={24} />
                    </div>
                ) : messages?.length === 0 ? (
                    <div className="flex flex-col items-center justify-center h-full text-center px-8">
                        <div className="w-16 h-16 rounded-3xl bg-indigo-500/10 flex items-center justify-center mb-6">
                            <Bot size={32} className="text-indigo-400" />
                        </div>
                        <p className="text-slate-400 font-medium text-sm leading-relaxed">
                            Start a conversation with Antigravity.<br />
                            Ask me to check your repo, run commands, or help with <span className="text-indigo-400 font-bold">{taskTitle}</span>.
                        </p>
                    </div>
                ) : (
                    messages?.map((msg) => (
                        <MessageBubble key={msg.id} message={msg} toolCall={parseToolCall(msg.suggestedAction)} />
                    ))
                )}
                <div ref={messagesEndRef} />
            </div>

            {/* Input */}
            <div className="p-4 bg-slate-900 border-t border-slate-700/50 shrink-0">
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
            </div>
        </div>
    );
}

function MessageBubble({ message, toolCall }: { message: TaskMessage; toolCall: AgentToolCall | null }) {
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

                {toolCall && (
                    <div className="mt-4 p-4 bg-slate-900/50 rounded-xl border border-slate-700/50">
                        <div className="flex items-center gap-2 text-[10px] font-black text-slate-400 uppercase tracking-[2px] mb-3">
                            <Terminal size={12} />
                            Suggested Command
                        </div>
                        <code className="block text-emerald-400 text-xs font-mono bg-slate-950 px-4 py-3 rounded-lg border border-slate-800">
                            {toolCall.command}
                        </code>
                        {toolCall.description && (
                            <p className="text-slate-500 text-[11px] mt-2">{toolCall.description}</p>
                        )}
                    </div>
                )}
            </div>
            {!isAgent && (
                <div className="w-8 h-8 rounded-xl bg-slate-700 flex items-center justify-center shrink-0 shadow-md">
                    <User size={16} className="text-slate-300" />
                </div>
            )}
        </div>
    );
}
