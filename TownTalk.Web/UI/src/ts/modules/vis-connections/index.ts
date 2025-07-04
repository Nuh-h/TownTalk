import $ from 'jquery';
import { Network } from 'vis-network/esnext';
import { ApiResponse, Node, Edge, Connection } from './interface';
import { DataSet } from 'vis-data/esnext';

/**
 * Fetches and visualizes the network of connections between two selected users.
 * Renders the network graph using vis.js and highlights mutual connections and shortest path.
 */
export async function GetVisNetwork(): Promise<void> {
    const $user1Select = $('form select#userId1');
    const $user2Select = $('form select#userId2');
    const user1 = $user1Select.val() as string;
    const user2 = $user2Select.val() as string;
    const user1Label = $user1Select.find('option:selected').text();
    const user2Label = $user2Select.find('option:selected').text();

    try {
        const data: ApiResponse = await $.get(`/connections?userId1=${user1}&userId2=${user2}`);

        $('#mutual-followers').html(`There are ${data.mutualConnections.length} mutual connections`);

        const nodes: Node[] = [];
        const edges: Edge[] = [];
        const nodeIds = new Set<string | number>();

        const addNode = (id: string | number, label: string, color: Node['color']) => {
            if (!nodeIds.has(id)) {
                nodes.push({ id, label, color });
                nodeIds.add(id);
            }
        };

        const addConnections = (userId: string | number, connections: Connection[]) => {
            connections.forEach((connection) => {
                addNode(connection.id, connection.displayName, { background: '#FF6F61', border: '#D32F2F' });
                edges.push({ from: userId, to: connection.id });
            });
        };

        const highlightPath = (path: { id: string | number }[], edges: Edge[]) => {
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
        };

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