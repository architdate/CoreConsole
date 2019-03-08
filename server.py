import subprocess
import os
import io
from flask import Flask, url_for, request, jsonify, send_file
app = Flask(__name__)

@app.route('/legalize', methods = ['POST'])
def api_legalize():
    if request.headers['Content-Type'] == 'application/octet-stream':
        with open('input.pkx', 'wb') as f:
            f.write(request.data)
        proc = subprocess.Popen(['CoreConsole.exe', '-alm', '--version', 'us', '-i', 'input.pkx'], stdout=subprocess.PIPE, shell=True)
        (out, err) = proc.communicate()
        fname = os.listdir(os.getcwd() + '/output')[0]
        with open(os.getcwd() + '/output/' + fname, 'rb') as f:
            pkx = f.read()
        os.remove(os.getcwd() + '/output/' + fname)
        return send_file(io.BytesIO(pkx), attachment_filename=fname)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)