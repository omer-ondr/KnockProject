# pip3 install sentence-transformers flask
# Çalıştır: python3 embedding_server.py
# Servis http://localhost:5500/embed adresinde dinler

from flask import Flask, request, jsonify
from sentence_transformers import SentenceTransformer

app = Flask(__name__)
model = SentenceTransformer('all-MiniLM-L6-v2')

@app.route('/embed', methods=['POST'])
def embed():
    data = request.get_json()
    text = data.get('inputs', '')
    embedding = model.encode(text).tolist()
    return jsonify(embedding)

if __name__ == '__main__':
    print("Embedding sunucusu http://localhost:5500 adresinde başlıyor...")
    app.run(host='0.0.0.0', port=5500, debug=False)
