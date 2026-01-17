'use client';

import { useState, useEffect, useRef } from 'react';

export interface LogLine {
    id: string;
    message: string;
    type: 'info' | 'error' | 'warn' | 'success';
    timestamp: string;
}

export function useLogs(attemptId: string | null) {
    const [logs, setLogs] = useState<LogLine[]>([]);
    const [status, setStatus] = useState<'connecting' | 'connected' | 'error' | 'closed'>('closed');
    const [prevAttemptId, setPrevAttemptId] = useState<string | null>(attemptId);
    const eventSourceRef = useRef<EventSource | null>(null);

    // Adjust state during render when attemptId changes (recommended React pattern)
    if (attemptId !== prevAttemptId) {
        setPrevAttemptId(attemptId);
        setLogs([]);
        setStatus(attemptId ? 'connecting' : 'closed');
    }

    useEffect(() => {
        if (!attemptId) return;

        const apiBase = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';
        const es = new EventSource(`${apiBase}/attempts/${attemptId}/stream`);
        eventSourceRef.current = es;

        es.onopen = () => setStatus('connected');

        es.addEventListener('log', (event: MessageEvent) => {
            const data = JSON.parse(event.data);
            setLogs(prev => [...prev, {
                id: Math.random().toString(36).substr(2, 9),
                message: data.message,
                type: data.level?.toLowerCase() || 'info',
                timestamp: new Date().toLocaleTimeString()
            }]);
        });

        es.onerror = (e) => {
            console.error('SSE Error:', e);
            setStatus('error');
            es.close();
        };

        return () => {
            es.close();
            eventSourceRef.current = null;
        };
    }, [attemptId]);

    return { logs, status, clear: () => setLogs([]) };
}
