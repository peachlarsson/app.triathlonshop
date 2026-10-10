import { useEffect, useState } from 'react';
import './App.css';

interface Product {
    id: number;
    name: string;
    unitPrice: number;
}

interface Customer {
    id: number;
    isGuest: boolean;
    name: string;
    email: string;
}

interface Order {
    id: number;
    customerId: number;
    orderNumber: string;
    totalPrice: number;
    orderRows: OrderRow[];
}

interface OrderRow {
    id: number;
    orderId: number;
    orderNumber: string;
    lineNumber: number;
    productId: number;
    productName: string;
    unitPrice: number;
    quantity: number;
}

function App() {
    const [products, setProducts] = useState<Product[]>();
    const [customers, setCustomers] = useState<Customer[]>();
    const [orders, setOrders] = useState<Order[]>();
    const [orderRows, setOrderRows] = useState<OrderRow[]>();

    useEffect(() => {
        getProducts();
        getCustomers();
        getOrders();
        getOrderRows();
    }, []);

    const productContents = products === undefined
        ? <p><em>Loading products...</em></p>
        : products.length === 0
            ? <p><em>No products found.</em></p>
            : <table className="table table-striped" aria-labelledby="productsTableLabel">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Price</th>
                    </tr>
                </thead>
                <tbody>
                    {products.map(product =>
                        <tr key={product.id}>
                            <td>{product.id}</td>
                            <td>{product.name}</td>
                            <td>{product.unitPrice.toFixed(2)}</td>
                        </tr>
                    )}
                </tbody>
            </table>;

    const customerContents = customers === undefined
        ? <p><em>Loading customers...</em></p>
        : customers.length === 0
            ? <p><em>No customers found.</em></p>
            : <table className="table table-striped" aria-labelledby="customersTableLabel">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>GuestCustomer</th>
                        <th>Name</th>
                        <th>Email</th>
                    </tr>
                </thead>
                <tbody>
                    {customers.map(customer =>
                        <tr key={customer.id}>
                            <td>{customer.id}</td>
                            <td>{customer.isGuest ? 'Yes' : 'No'}</td>
                            <td>{customer.name}</td>
                            <td>{customer.email}</td>
                        </tr>
                    )}
                </tbody>
            </table>;

    const orderContents = orders === undefined
        ? <p><em>Loading orders...</em></p>
        : orders.length === 0
            ? <p><em>No orders found.</em></p>
            : <table className="table table-striped" aria-labelledby="ordersTableLabel">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>CustomerId</th>
                        <th>OrderNumber</th>
                        <th>TotalPrice</th>
                    </tr>
                </thead>
                <tbody>
                    {orders.map(order =>
                        <tr key={order.id}>
                            <td>{order.id}</td>
                            <td>{order.customerId}</td>
                            <td>{order.orderNumber}</td>
                            <td>{order.totalPrice.toFixed(2)}</td>
                        </tr>
                    )}
                </tbody>
            </table>;

    const orderRowContents = orderRows === undefined
        ? <p><em>Loading order rows...</em></p>
        : orderRows.length === 0
            ? <p><em>No order rows found.</em></p>
            : <table className="table table-striped" aria-labelledby="orderRowsTableLabel">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>OrderId</th>
                        <th>OrderNumber</th>
                        <th>LineNumber</th>
                        <th>ProductId</th>
                        <th>ProductName</th>
                        <th>UnitPrice</th>
                        <th>Quantity</th>
                    </tr>
                </thead>
                <tbody>
                    {orderRows.map(orderRow =>
                        <tr key={orderRow.id}>
                            <td>{orderRow.id}</td>
                            <td>{orderRow.orderId}</td>
                            <td>{orderRow.orderNumber}</td>
                            <td>{orderRow.lineNumber}</td>
                            <td>{orderRow.productId}</td>
                            <td>{orderRow.productName}</td>
                            <td>{orderRow.unitPrice.toFixed(2)}</td>
                            <td>{orderRow.quantity}</td>
                        </tr>
                    )}
                </tbody>
            </table>;

    return (
        <div>
            <h1 id="productsTableLabel">Products</h1>
            <p>This component demonstrates fetching product data from the server.</p>
            {productContents}

            <h1 id="customersTableLabel">Customers</h1>
            <p>This component demonstrates fetching customer data from the server.</p>
            {customerContents}

            <h1 id="ordersTableLabel">Orders</h1>
            <p>This component demonstrates fetching order data from the server.</p>
            {orderContents}

            <h1 id="orderRowsTableLabel">Order Rows</h1>
            <p>This component demonstrates fetching order row data from the server.</p>
            {orderRowContents}
        </div>
    );

    async function getProducts() {

        const response = await fetch('product');

        if (response.ok)
        {
            const data: Product[] = await response.json();
            setProducts(data);
        }
        else
        {
            console.error('Failed to fetch products:', response.status, response.statusText);
        }
    }

    async function getCustomers() {

        const response = await fetch('customer');

        if (response.ok)
        {
            const data: Customer[] = await response.json();
            setCustomers(data);
        }
        else
        {
            console.error('Failed to fetch customers:', response.status, response.statusText);
        }
    }

    async function getOrders() {

        const response = await fetch('order');

        if (response.ok)
        {
            const data: Order[] = await response.json();
            setOrders(data);
        }
        else
        {
            console.error('Failed to fetch orders:', response.status, response.statusText);
        }
    }

    async function getOrderRows() {

        const response = await fetch('orderRow');

        if (response.ok)
        {
            const data: OrderRow[] = await response.json();
            setOrderRows(data);
        }
        else
        {
            console.error('Failed to fetch order rows:', response.status, response.statusText);
        }
    }
}

export default App;