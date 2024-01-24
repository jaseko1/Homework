'use client'

import React, { createContext, useState, useEffect } from 'react';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import * as signalR from "@microsoft/signalr";

interface ISignalRContext {
    connection: HubConnection | null;
}

export const SignalRContext = createContext<ISignalRContext>({ connection: null });

export const SignalRProvider: React.FC = ({ children }) => {
    const [connection, setConnection] = useState<HubConnection | null>(null);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5289/notificationHub")
            .build();

        setConnection(newConnection);

        try {
            newConnection.start()
                .then(() => {
                    console.log("Connected to the notification hub.");
                })
                .catch(err => console.error('Error while establishing connection: ', err));
        } catch (err) {
            console.error("Error connecting to SignalR hub:", err);
        }

        return () => {
            newConnection.stop();
        };
    }, []);

    return (
        <SignalRContext.Provider value={{ connection }}>
            {children}
        </SignalRContext.Provider>
    );
};
