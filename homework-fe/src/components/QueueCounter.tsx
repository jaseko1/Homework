'use client'

import React, { useEffect, useState } from 'react';
import * as signalR from "@microsoft/signalr";

const QueueCounter = () => {
    const [queueCount, setQueueCount] = useState(0);
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5289/notificationHub")
            .build();

        connection.start()
            .then(() => {
                console.log("Connected to the notification hub.");
                connection.on("ReceiveQueueCount", (count) => {
                    setQueueCount(count);
                });
            })
            .catch(err => console.error('Error while establishing connection: ', err));

        return () => {
            connection.stop();
        };
    }, []);

    return (
        <div className="z-10 max-w-5xl w-full font-mono text-sm">
            <p className="w-full border-b border-gray-300 bg-gradient-to-b from-zinc-200 pb-6 pt-8 backdrop-blur-2xl dark:border-neutral-800 dark:bg-zinc-800/30 dark:from-inherit rounded-xl border bg-gray-200 p-4 dark:bg-zinc-800/30">
                Aktuálně čeká ve frontě &nbsp;
                <code className="font-mono font-bold">{queueCount} requestů</code>
            </p>
        </div>
    );
};

export default QueueCounter;
