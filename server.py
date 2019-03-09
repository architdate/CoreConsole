import subprocess
import os
import io
import uuid
from datetime import datetime
from flask import Flask, url_for, request, jsonify, send_file

app = Flask(__name__)

DIR_OUTPUT = "output"

def ensureFolderExists(folder):
    if not os.path.exists(folder):
        os.makedirs(folder)

def datePath():
    return datetime.now().strftime("%Y%m%d")

@app.route('/api/legalize', methods = ['POST'])
def api_legalize():
    try:
        if request.headers['Content-Type'] == 'application/octet-stream':
            if not request.data or not request.headers.get('Version'):
                print("shit")
                return None
            storeFolder = os.path.join(DIR_OUTPUT, datePath())
            ensureFolderExists(storeFolder)
            inputPath = os.path.join(storeFolder, str(uuid.uuid4()) + ".pkx")
            with open(inputPath, 'wb') as f:
                f.write(request.data)
            proc = subprocess.Popen(['CoreConsole/bin/Debug/CoreConsole.exe', '-alm', '--version', request.headers.get('Version'), '-i', inputPath], stdout=subprocess.PIPE, shell=True)
            (out, err) = proc.communicate()
            with open(inputPath, 'rb') as f:
                pkx = f.read()
            return send_file(io.BytesIO(pkx), attachment_filename="output.pkx")
    except Exception as e:
        print(e)
        return None

if __name__ == '__main__':
    ensureFolderExists(DIR_OUTPUT)
    app.run(host='0.0.0.0', port=5000)