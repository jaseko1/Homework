'use client'

import React, { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export default function NotificationHub() {
    const [notifications, setNotifications] = useState([]);
    const [input, setInput] = useState('');
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5289/notificationHub")
            .build();

        connection.start()
            .then(() => {
                console.log("Connected to the notification hub.");
                connection.on("ReceiveNotification", message => {
                    setNotifications(prev => [...prev, message]);
                });
            })
            .catch(err => console.error('Error while establishing connection: ', err));

        return () => {
            connection.stop();
        };
    }, []);

    const handleInputChange = (e) => {
        setInput(e.target.value);
    };

    const sendCommand = () => {
        if (input.trim() === '/clear') {
            setNotifications(['Console cleared']);
        } else {
            // Zde můžete přidat logiku pro zpracování ostatních příkazů
            setNotifications([...notifications, input]);
        }
        setInput('');
    };

    const handleKeyPress = (e) => {
        if (e.key === 'Enter') {
            sendCommand();
        }
    };

    return (
        <div className="scrollbar bg-gray-900 text-green-400 font-mono p-4 rounded-lg max-h-30 shadow-lg w-2/3 overflow-y-auto">
                <p className="text-sm mb-2">NotificationHub&gt;</p>

                {/* Příklad notifikace */}
                <div className="mb-2 flex flex-col space-y-2">
                    {notifications.map((notification, index) => (
                        <div key={index} className="bg-blue-500/30 border border-blue-500 text-blue-500 p-2 rounded-md shadow">
                            <p className="text-xs">{notification}</p>
                        </div>
                    ))}
                </div>

                {/* Další obsah konzole */}

                <div className="mt-4 flex">
                    <input
                        type="text"
                        value={input}
                        onChange={handleInputChange}
                        onKeyPress={handleKeyPress}
                        className="bg-gray-800 text-green-400 p-2 w-full rounded-l focus:outline-none"
                        placeholder="Zadejte příkaz..."
                    />
                    <button
                        onClick={sendCommand}
                        className="bg-blue-600 text-white font-bold py-2 px-4 rounded-r hover:bg-blue-700 focus:outline-none focus:shadow-outline"
                    >
                        Odeslat
                    </button>
                </div>
        </div>
    );
}
