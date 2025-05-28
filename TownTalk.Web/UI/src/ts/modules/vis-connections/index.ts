import $ from 'jquery';
import { Network } from 'vis-network/esnext';
import { ApiResponse, Node, Edge, Connection } from './interface';
import { DataSet } from 'vis-data/esnext';

/**
 * Fetches and visualizes the network of connections between two selected users.
 *
 * This function retrieves the selected user IDs and labels from the DOM, fetches their connection data
 * from the server, and renders a network graph using the vis.js library. The graph displays the users,
 * their mutual connections, and highlights the shortest path between them if available.
 *
 * The visualization is rendered inside the DOM element with the ID 'network'. Node and edge styles are
 * customized for clarity and emphasis, including special highlighting for mutual connections and the
 * shortest path.
 *
 * @async
 * @function
 * @returns {Promise<void>} A promise that resolves when the network has been rendered.
 * @throws Will log an error to the console if fetching or rendering fails.
 */
export async function GetVisNetwork(): Promise<void> {
    const user1 = $('form select#userId1').val() as string;
    const user1Label = $('form select#userId1 option:selected').text();
    const user2 = $('form select#userId2').val() as string;
    const user2Label = $('form select#userId2 option:selected').text();

    try {
        const data: ApiResponse = await $.get(`/connections?userId1=${user1}&userId2=${user2}`);

        $('#mutual-followers').html(`There are ${data.mutualConnections.length} mutual connections`);

        const nodes: Node[] = [];
        const edges: Edge[] = [];
        const nodeIds = new Set<string | number>();

        function addNode(id: string | number, label: string, color: Node['color']) {
            if (!nodeIds.has(id)) {
                nodes.push({ id, label, color });
                nodeIds.add(id);
            }
        }

        function addConnections(userId: string | number, connections: Connection[]) {
            connections.forEach((connection) => {
                addNode(connection.id, connection.displayName, { background: '#FF6F61', border: '#D32F2F' });
                edges.push({ from: userId, to: connection.id });
            });
        }

        function highlightPath(path: { id: string | number }[], edges: Edge[]) {
            for (let i = 0; i < path.length - 1; i++) {
                const fromNode = path[i].id;
                const toNode = path[i + 1].id;
                const edge = edges.find(
                    (e) => (e.from === fromNode && e.to === toNode) || (e.from === toNode && e.to === fromNode)
                );
                if (edge) {
                    edge.color = { color: '#FF8C00' };
                    edge.width = 4;
                }
            }
        }

        addNode(user1, user1Label, { background: '#FF0000', border: '#D32F2F' });
        addNode(user2, user2Label, { background: '#00FF00', border: '#388E3C' });

        data.mutualConnections.forEach((follower) => {
            addNode(follower.id, follower.displayName, { background: '#318E3C', border: '#FF9800' });
            edges.push({ from: user1, to: follower.id });
            edges.push({ from: user2, to: follower.id });
        });

        addConnections(user1, data.allConnectionsUser1);
        addConnections(user2, data.allConnectionsUser2);
        highlightPath(data.path, edges);

        const container = document.getElementById('network');
        if (!container) throw new Error("Container with id 'network' not found.");

        const networkData: any = {
            nodes: new DataSet(nodes),
            edges: new DataSet(edges as any),
        };

        const options = {
            nodes: {
                shape: 'dot',
                size: 30,
                color: {
                    background: '#FF6F61',
                    border: '#D32F2F',
                    highlight: {
                        background: '#FF8A65',
                        border: '#C2185B',
                    },
                },
                font: {
                    color: 'white',
                    size: 16,
                    face: 'arial',
                },
            },
            edges: {
                width: 2,
                color: '#A9A9A9',
                smooth: true,
            },
            physics: {
                enabled: true,
            },
        };

        new Network(container, networkData, options);
    } catch (error) {
        console.error('Failed to fetch and render network:', error);
    }
}
