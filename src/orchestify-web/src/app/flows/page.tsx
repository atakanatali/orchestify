'use client';

import { useState } from 'react';
import { Zap, ExternalLink, Maximize2, Minimize2, RefreshCw, Loader2, AlertTriangle } from 'lucide-react';
import clsx from 'clsx';

export default function FlowsPage() {
    const [isFullscreen, setIsFullscreen] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [hasError, setHasError] = useState(false);
    const flowsUrl = 'http://localhost:5678';

    const handleRefresh = () => {
        setIsLoading(true);
        setHasError(false);
        const iframe = document.querySelector('iframe') as HTMLIFrameElement;
        if (iframe) {
            iframe.src = flowsUrl;
        }
    };

    return (
        <div className={clsx(
            "flex flex-col bg-white overflow-hidden",
            isFullscreen ? "fixed inset-0 z-[200]" : "h-full"
        )}>
            {/* Header */}
            <div className="px-8 py-4 border-b border-slate-100 flex items-center justify-between shrink-0">
                <div className="flex items-center gap-4">
                    <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-orange-500 to-amber-600 flex items-center justify-center shadow-lg shadow-orange-500/20">
                        <Zap size={20} className="text-white" />
                    </div>
                    <div>
                        <h1 className="text-xl font-black text-slate-900 tracking-tight">Flows</h1>
                        <p className="text-slate-400 text-xs font-medium">Workflow Automation</p>
                    </div>
                </div>

                <div className="flex items-center gap-2">
                    <button
                        onClick={handleRefresh}
                        className="p-2.5 bg-slate-100 text-slate-600 rounded-lg hover:bg-slate-200 transition-all"
                        title="Refresh"
                    >
                        <RefreshCw size={16} className={isLoading ? 'animate-spin' : ''} />
                    </button>
                    <button
                        onClick={() => setIsFullscreen(!isFullscreen)}
                        className="p-2.5 bg-slate-100 text-slate-600 rounded-lg hover:bg-slate-200 transition-all"
                        title={isFullscreen ? 'Exit Fullscreen' : 'Fullscreen'}
                    >
                        {isFullscreen ? <Minimize2 size={16} /> : <Maximize2 size={16} />}
                    </button>
                    <a
                        href={flowsUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="p-2.5 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-all shadow-lg shadow-indigo-500/20"
                        title="Open in New Tab"
                    >
                        <ExternalLink size={16} />
                    </a>
                </div>
            </div>

            {/* iframe Container */}
            <div className="flex-1 relative">
                {/* Loading State */}
                {isLoading && !hasError && (
                    <div className="absolute inset-0 flex flex-col items-center justify-center bg-slate-50 z-10">
                        <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-orange-500 to-amber-600 flex items-center justify-center mb-4 shadow-xl">
                            <Loader2 size={28} className="text-white animate-spin" />
                        </div>
                        <p className="text-slate-600 font-bold text-sm">Loading Workflow Editor...</p>
                    </div>
                )}

                {/* Error State */}
                {hasError && (
                    <div className="absolute inset-0 flex flex-col items-center justify-center bg-slate-50 z-10">
                        <div className="w-14 h-14 rounded-2xl bg-amber-100 flex items-center justify-center mb-4">
                            <AlertTriangle size={28} className="text-amber-600" />
                        </div>
                        <p className="text-slate-600 font-bold text-sm mb-2">Connection Failed</p>
                        <p className="text-slate-400 text-xs mb-4">Make sure the workflow engine is running</p>
                        <button
                            onClick={handleRefresh}
                            className="px-4 py-2 bg-indigo-600 text-white rounded-lg text-xs font-bold"
                        >
                            Retry
                        </button>
                    </div>
                )}

                {/* Workflow Editor iframe */}
                <iframe
                    src={flowsUrl}
                    className="w-full h-full border-0"
                    onLoad={() => {
                        setIsLoading(false);
                        setHasError(false);
                    }}
                    onError={() => {
                        setIsLoading(false);
                        setHasError(true);
                    }}
                    title="Workflow Editor"
                    allow="clipboard-read; clipboard-write"
                />
            </div>
        </div>
    );
}
