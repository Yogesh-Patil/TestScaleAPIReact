import React, {useEffect, useState} from "react";

export default function App(){
  const [list,setList] = useState([]);
  useEffect(()=> { fetch('/api/products?page=1&pageSize=10').then(r=>r.json()).then(setList)},[]);
  return <div style={{padding:20}}>
    <h1>PerfDemo Products</h1>
    <ul>{list.map(p=> <li key={p.id}>{p.id} - {p.name}</li>)}</ul>
  </div>
}