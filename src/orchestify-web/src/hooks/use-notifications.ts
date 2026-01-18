'use client';

import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';

const HUB_URL = process.env.NEXT_PUBLIC_HUB_URL || 'http://localhost:5001/hubs/task-execution';

export function useNotifications() {
    const queryClient = useQueryClient();
    const connectionRef = useRef<signalR.HubConnection | null>(null);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL)
            .withAutomaticReconnect()
            .build();

        connectionRef.current = connection;

        const startConnection = async () => {
            try {
                await connection.start();
                console.log('SignalR Connected.');
            } catch (err) {
                console.error('SignalR Connection Error: ', err);
                setTimeout(startConnection, 5000);
            }
        };

        connection.on('TaskStatusChanged', (taskId: string, status: string) => {
            console.log(`Task ${taskId} status changed to ${status}`);
            queryClient.invalidateQueries({ queryKey: ['tasks'] });
        });

        connection.on('AttemptStatusChanged', (attemptId: string, state: string) => {
            console.log(`Attempt ${attemptId} state changed to ${state}`);
            queryClient.invalidateQueries({ queryKey: ['tasks'] });
            queryClient.invalidateQueries({ queryKey: ['dashboard-stats'] });
        });

        connection.on('StepStatusChanged', (stepId: string, state: string) => {
            console.log(`Step ${stepId} status changed to ${state}`);
            // Usually invalidate steps too if modal is open
        });

        startConnection();

        return () => {
            connection.stop();
        };
    }, [queryClient]);

    const joinTask = async (taskId: string) => {
        if (connectionRef.current?.state === signalR.HubConnectionState.Connected) {
            await connectionRef.current.invoke('JoinTaskGroup', taskId);
        }
    };

    const leaveTask = async (taskId: string) => {
        if (connectionRef.current?.state === signalR.HubConnectionState.Connected) {
            await connectionRef.current.invoke('LeaveTaskGroup', taskId);
        }
    };

    return { joinTask, leaveTask };
}
