import http from 'k6/http';
import { sleep, check } from 'k6';

export let options = {
  stages: [
    { duration: '1m', target: 10 },
    { duration: '2m', target: 50 },
    { duration: '3m', target: 200 },
    { duration: '5m', target: 500 },
    { duration: '5m', target: 1000 },
    { duration: '5m', target: 0 }
  ],
  thresholds: {
    http_req_duration: ['p(95)<3000'],
    'http_req_failed': ['rate<0.05']
  }
};

export default function () {
  let res = http.get('http://api/api/products?page=1&pageSize=10');
  check(res, { 'status was 200': (r) => r.status === 200 });
  sleep(1);
}
