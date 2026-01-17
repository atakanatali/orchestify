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

    // Adjust state during render when attemptId changes
    if (attemptId !== prevAttemptId) {
        setPrevAttemptId(attemptId);
        setLogs([]);
        setStatus(attemptId ? 'connecting' : 'closed');
    }

    useEffect(() => {
        if (!attemptId) return;

        const apiBase = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

        // Add initial connecting log
        setLogs([{
            id: 'init',
            message: `Connecting to attempt stream: ${attemptId.split('-')[0]}...`,
            type: 'info',
            timestamp: new Date().toLocaleTimeString()
        }]);

        try {
            const es = new EventSource(`${apiBase}/attempts/${attemptId}/stream`);
            eventSourceRef.current = es;

            es.onopen = () => {
                setStatus('connected');
                setLogs(prev => [...prev, {
                    id: 'connected',
                    message: 'Stream connected successfully',
                    type: 'success',
                    timestamp: new Date().toLocaleTimeString()
                }]);
            };

            es.addEventListener('log', (event: MessageEvent) => {
                try {
                    const data = JSON.parse(event.data);
                    setLogs(prev => [...prev, {
                        id: Math.random().toString(36).substr(2, 9),
                        message: data.message,
                        type: data.level?.toLowerCase() || 'info',
                        timestamp: new Date().toLocaleTimeString()
                    }]);
                } catch (parseError) {
                    console.error('Log parse error:', parseError);
                }
            });

            es.onerror = () => {
                setStatus('error');
                setLogs(prev => [...prev, {
                    id: 'error',
                    message: 'Stream connection failed or closed. The attempt may still be processing.',
                    type: 'warn',
                    timestamp: new Date().toLocaleTimeString()
                }]);
                es.close();
            };

            return () => {
                es.close();
                eventSourceRef.current = null;
            };
        } catch (error) {
            setStatus('error');
            setLogs(prev => [...prev, {
                id: 'catch-error',
                message: `Failed to connect: ${error instanceof Error ? error.message : 'Unknown error'}`,
                type: 'error',
                timestamp: new Date().toLocaleTimeString()
            }]);
        }
    }, [attemptId]);

    return { logs, status, clear: () => setLogs([]) };
}
