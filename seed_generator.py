import json
from sentence_transformers import SentenceTransformer

# Tamamen ücretsiz ve bilgisayarında çalışan model (384 boyutlu vektör üretir)
model = SentenceTransformer('all-MiniLM-L6-v2')

raw_data = [
    {
        "textContent": "Anne, bu rozeti benden al. Artık kullanamıyorum. Etrafım çok kararıyor, göremiyorum. Sanki cennetin kapısını çalıyormuşum gibi hissediyorum.",
        "source": "Bob Dylan - Knockin' on Heaven's Door (1973)",
        "year": 1973
    },
    {
        "textContent": "Burada yağmur hiç durmuyor. Botlarımın içindeki çamura alıştım ama içimdeki bu boşluğa alışamıyorum. Dün radyoda bir şarkı çaldı, eve dönmenin ne kadar uzak olduğunu bir kez daha anladım. Silahımı her elime aldığımda kendimden bir parça daha kaybediyorum.",
        "source": "Anonim Vietnam Asker Mektubu",
        "year": 1973
    },
    {
        "textContent": "Şerif yavaşça doğruldu. Gözlerindeki o eski, acımasız parıltı gitmişti. Yıllarca taşıdığı, kan ve tozla kirlenmiş teneke rozeti göğsünden söküp ahşap masanın üzerine bıraktı. Rozet masaya çarptığında çıkan o tok ses, bir devrin kapanışını ilan ediyordu.",
        "source": "Pat Garrett & Billy the Kid (1973) - Atmosferik Tasvir",
        "year": 1973
    },
    {
        "textContent": "Savaşmak istemiyoruz. Bize ait olmayan bir ormanda, tanımadığımız insanları vurmak istemiyoruz. Üniformalarımızı çıkarıp sadece insan olmak istiyoruz. Barış, namlunun ucunda değil, o namluyu yere bırakabilme cesaretindedir.",
        "source": "1973 Savaş Karşıtı Protesto Sloganı",
        "year": 1973
    },
    {
        "textContent": "Bir zamanlar inandığım tüm doğrular şimdi bu tozlu yolda rüzgarla savruluyor. Kanun dedikleri şey, güçlülerin zayıfları ezmek için uydurduğu bir masalmış. Rozetimi çıkarıyorum çünkü artık bu masalın bir parçası olmak istemiyorum.",
        "source": "Western Kasabası Reddediş Mektubu (Kurgusal 1973 Konsepti)",
        "year": 1973
    }
]

def generate_embeddings_and_save():
    print("Model hazırlanıyor ve vektörler oluşturuluyor, lütfen bekleyin...")
    seed_data = []
    
    for item in raw_data:
        try:
            # Metni lokal modelle vektörize et (API key gerekmez)
            embedding = model.encode(item["textContent"]).tolist()
            
            seed_data.append({
                "textContent": item["textContent"],
                "source": item["source"],
                "year": item["year"],
                "embedding": embedding
            })
            print(f"Başarılı: {item['source']}")
            
        except Exception as e:
            print(f"Hata oluştu ({item['source']}): {str(e)}")

    with open('seed.json', 'w', encoding='utf-8') as f:
        json.dump(seed_data, f, ensure_ascii=False, indent=2)
    
    print("\nHarika! 384 boyutlu 'seed.json' dosyası başarıyla oluşturuldu.")

if __name__ == "__main__":
    generate_embeddings_and_save()