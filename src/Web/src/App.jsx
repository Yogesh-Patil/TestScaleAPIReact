import React, { useEffect, useState } from "react";

const PayloadCell = ({ payload }) => {
  const [showTooltip, setShowTooltip] = useState(false);
  const [copied, setCopied] = useState(false);
  const maxLength = 50;
  const truncated = payload.length > maxLength ? payload.substring(0, maxLength) + '...' : payload;

  const handleCopy = () => {
    navigator.clipboard.writeText(payload);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <span style={{ position: 'relative', display: 'inline-block' }}>
      <span
        style={{
          cursor: payload.length > maxLength ? 'help' : 'default',
          textDecoration: payload.length > maxLength ? 'underline dotted' : 'none'
        }}
        onMouseEnter={() => setShowTooltip(true)}
        onMouseLeave={() => setShowTooltip(false)}
      >
        {truncated}
      </span>

      {showTooltip && payload.length > maxLength && (
        <div style={{
          position: 'absolute',
          bottom: '100%',
          left: '0',
          backgroundColor: '#2d2d2d',
          color: '#fff',
          padding: '12px',
          borderRadius: '6px',
          boxShadow: '0 4px 12px rgba(0,0,0,0.3)',
          zIndex: 1000,
          minWidth: '300px',
          maxWidth: '500px',
          marginBottom: '8px',
          wordBreak: 'break-all'
        }}>
          <div style={{ marginBottom: '8px', fontSize: '13px', lineHeight: '1.5' }}>
            {payload}
          </div>
          <button
            onClick={handleCopy}
            style={{
              backgroundColor: copied ? '#10b981' : '#3b82f6',
              color: 'white',
              border: 'none',
              padding: '6px 12px',
              borderRadius: '4px',
              cursor: 'pointer',
              fontSize: '12px',
              fontWeight: '500'
            }}
          >
            {copied ? '✓ Copied!' : 'Copy'}
          </button>
          {/* Arrow */}
          <div style={{
            position: 'absolute',
            bottom: '-6px',
            left: '20px',
            width: '0',
            height: '0',
            borderLeft: '6px solid transparent',
            borderRight: '6px solid transparent',
            borderTop: '6px solid #2d2d2d'
          }}></div>
        </div>
      )}
    </span>
  );
};

export default function App() {
  const [list, setList] = useState([]);
  useEffect(() => { fetch('/api/products?page=1&pageSize=10').then(r => r.json()).then(setList) }, []);
  return <div style={{ padding: 20 }}>
    <h1>PerfDemo Products</h1>
    <ul style={{ lineHeight: '2' }}>
      {list.map(p => (
        <li key={p.id}>
          {p.id} - {p.name} - <PayloadCell payload={p.payload} />
        </li>
      ))}
    </ul>
  </div>
}