import requests

with open('test.pk7', 'rb') as f:
    data = f.read()

res = requests.post(url='http://118.200.51.43:5000/legalize', data=data, headers={'Content-Type': 'application/octet-stream'})

with open('out.pk7', 'wb') as f:
    f.write(res.content)