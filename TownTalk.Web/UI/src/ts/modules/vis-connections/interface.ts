export type Node = {
    id: string | number;
    label: string;
    color: {
        background: string;
        border: string;
    };
};
export type Edge = {
    from: string | number;
    to: string | number;
    color?: { color: string; };
    width?: number;
};
export type Connection = {
    id: string | number;
    displayName: string;
};
export type ApiResponse = {
    mutualConnections: Connection[];
    allConnectionsUser1: Connection[];
    allConnectionsUser2: Connection[];
    path: { id: string | number; }[];
};
