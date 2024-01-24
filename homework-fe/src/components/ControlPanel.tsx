'use client'

import React from 'react';

const ControlPanel = () => {
    const fetchCustomers = async () => {
        const url = 'http://localhost:5289/Customer?pageSize=20&pageNumber=1';

        try {
            const response = await fetch(url);
            const data = await response.json();
            return data.results;
        } catch (error) {
            console.error('Error fetching customers:', error);
        }
    };

    const sendPutRequests = async () => {
        const customers = await fetchCustomers();

        if (customers && customers.length > 0) {
            customers.slice(0, 20).forEach(async (customer) => {
                const putUrl = `http://localhost:5289/Customer`;
                const updatedData = {
                    id: customer.id,
                    firstName: "UpdatedFirstName",
                    lastName: "UpdatedLastName",
                    phone: "UpdatedPhone",
                    email: "updated@email.com"
                };

                try {
                    const response = await fetch(putUrl, {
                        method: 'PUT',
                        headers: {
                            'Content-Type': 'application/json',
                        },
                        body: JSON.stringify(updatedData),
                    });
                    const result = await response.json();
                    console.log('Updated:', result);
                } catch (error) {
                    console.error('Error updating customer:', error);
                }
            });
        }
    };


    const sendDeleteRequests = async () => {
        const customers = await fetchCustomers();

        if (customers && customers.length > 0) {
            customers.slice(0, 20).forEach(async (customer) => {
                const deleteUrl = `http://localhost:5289/Customer/${customer.id}`;

                try {
                    const response = await fetch(deleteUrl, { method: 'DELETE' });
                    const result = await response.json();
                    console.log('Deleted:', result);
                } catch (error) {
                    console.error('Error deleting customer:', error);
                }
            });
        }
    };

    const sendData = async () => {
        const url = 'http://localhost:5289/Customer';
        const data = {
            firstName: "Jan",
            lastName: "Test",
            phone: "454645454",
            email: "string@asd.cz"
        };

        for (let i = 0; i < 20; i++) {
            fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            })
                .then(response => response.json())
                .then(data => {
                    console.log('Success:', data);
                })
                .catch((error) => {
                    console.error('Error:', error);
                });
        }
    };

    return (
        <div className="bg-gray-900 p-4 rounded-lg shadow-lg">
            <div className="grid grid-cols-3 gap-3">
                <button onClick={sendPutRequests} className="bg-blue-600 text-white font-bold py-2 px-4 rounded hover:bg-blue-700 focus:outline-none focus:shadow-outline transform hover:scale-105 transition duration-300 ease-in-out">
                    Send 20x Update
                </button>
                <button onClick={sendData} className="bg-green-600 text-white font-bold py-2 px-4 rounded hover:bg-green-700 focus:outline-none focus:shadow-outline transform hover:scale-105 transition duration-300 ease-in-out">
                    Send 20x Create
                </button>
                <button onClick={sendDeleteRequests} className="bg-red-600 text-white font-bold py-2 px-4 rounded hover:bg-red-700 focus:outline-none focus:shadow-outline transform hover:scale-105 transition duration-300 ease-in-out">
                    Send 20x Delete
                </button>
            </div>
        </div>
    );
};

export default ControlPanel;
