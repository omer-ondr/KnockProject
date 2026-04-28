import json
from sentence_transformers import SentenceTransformer

# Tamamen ücretsiz ve bilgisayarında çalışan model (384 boyutlu vektör üretir)
model = SentenceTransformer('all-MiniLM-L6-v2')

raw_data = [
    # ── Orijinal 5 kayıt ────────────────────────────────────────────────────
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
    },

    # ── Vietnam Savaşı Mektupları ─────────────────────────────────────────────
    {
        "textContent": "Orman burada hiç bitmez gibi görünüyor. Gündüz yaprakların arasından süzülen güneş sizi aldatır; gece ise her yer karanlık bir mezara dönüşür. Yanımdaki çocuk on sekiz yaşındaydı, dün sabah gözlerini açamadı. Düşmanı öldürmek değil, öldürmeden eve dönmek istiyoruz artık.",
        "source": "Vietnam Savaşı - James R. Holloway Mektubu (1973)",
        "year": 1973
    },
    {
        "textContent": "Sana söylemesi en zor şeyi yazıyorum: Artık neye inandığımı bilmiyorum. Burada öldürmenin haklı olduğunu söylediler. Ama her gece uyumadan önce gözlerimi kapattığımda yalnızca yanmış köyleri ve ağlayan çocukları görüyorum. Bizi eve gönder.",
        "source": "Vietnam - Anonim Asker Günlüğü, Saigon 1973",
        "year": 1973
    },
    {
        "textContent": "Silah sesi duyulduğunda herkes yere yatıyordu. Bir refleks artık, düşünmeden yapıyoruz. Ama asıl korkutucu olan sessizlik — ateş bittiğinde ne kadar azaldığımızı sayan o uzun, boş sessizlik.",
        "source": "Vietnam - Piyade Mektubu, Da Nang 1973",
        "year": 1973
    },
    {
        "textContent": "Nehir üzerindeki sisle birlikte her sabah biraz daha kayboluyorum. Botlarım çürüdü, üniformam renk değiştirdi, ama en kötüsü aynamın içindeki adamın gözlerini tanıyamamam. O adam ben değilim artık.",
        "source": "Vietnam - Delta Bölgesi Asker Mektubu, 1973",
        "year": 1973
    },

    # ── Savaş Karşıtı Hareket / Hippie Kültürü ───────────────────────────────
    {
        "textContent": "Washington meydanında beş yüz bin kişiydik. Şarkı söyledik, bağırdık, ağladık. Ama akşam haberlerinde bizi görmediler, yalnızca kırılan camları gösterdiler. Barış hareketi can çekişiyor dedi gazeteci; hayır, sistem can çekişiyordu.",
        "source": "1973 Barış Yürüyüşü - Katılımcı Tanıklığı",
        "year": 1973
    },
    {
        "textContent": "Çiçekler silahlardan güçlüdür dedik ve inandık. Belki hâlâ inanıyoruz. Ama o yıl çok fazla arkadaşımızı kaybettik — kimileri Vietnam'da, kimileri burada kaldırımlarda ve hapishane koridorlarında.",
        "source": "Hippie Manifestosu Fragmanı - San Francisco 1973",
        "year": 1973
    },
    {
        "textContent": "Askerlik çağrısı geldiğinde kartı yaktım. Annem ağladı, komşular dönüp baktı. Ama içimde garip bir özgürlük hissettim, sanki zincirlerimi kendim kırmıştım. Özgürlük bazen çok ağır bir bedel ister.",
        "source": "Askerlik Kartı Yakma Eylemi - 1973 Tanıklığı",
        "year": 1973
    },

    # ── Bob Dylan / Folk Melankolisi ─────────────────────────────────────────
    {
        "textContent": "Yollar sonunda ayrılıyor ve sen hangi yolun seni eve götüreceğini bilemiyorsun. Dylan'ın sesi radyodan yükseldiğinde o anda anlıyorsun ki bazı kapılar bir kez kapandıktan sonra bir daha açılmıyor.",
        "source": "Bob Dylan - Blowin' in the Wind, 1973 Sahne Notu",
        "year": 1973
    },
    {
        "textContent": "Cevap rüzgarda savruluyor dedi. Gerçekten savruluyor; bulmak için yalnızca elini uzatman gerekiyor ama rüzgar her seferinde yönünü değiştiriyor. Belki cevap diye bir şey yoktur, belki yalnızca soru vardır.",
        "source": "Bob Dylan - Konser Sonrası Backstage Notu, 1973",
        "year": 1973
    },
    {
        "textContent": "Gitar teli koptu tam ortasında, o hüzünlü nakarat çıkarken. Hiç durmadı, bozuk telle çalmaya devam etti. Mükemmelliğin değil, gerçekliğin sesi buydu — çatlak, ham ve dürüst.",
        "source": "Bob Dylan - Madison Square Garden 1973, Görgü Tanığı",
        "year": 1973
    },

    # ── Pat Garrett / Western İhanet ─────────────────────────────────────────
    {
        "textContent": "Pat Garrett eski dostunun izini sürerken içinde bir şeyin kırıldığını hissediyordu. Kanun ve dostluk aynı anda tutulamazdı bu çölde. Rozetinin ağırlığı her geçen gün artıyordu, sanki içi kurşunla doluymuş gibi.",
        "source": "Pat Garrett & Billy the Kid - Senaryo Notu, Sam Peckinpah 1973",
        "year": 1973
    },
    {
        "textContent": "Billy öldüğünde Garrett ayakta duruyordu, silahı hâlâ elindeydi. Kazanmıştı. Ama o gece dışarı çıkıp kusmak zorunda kaldı. Zafer dedikleri buysa eğer, bir daha kazanmak istemiyordu.",
        "source": "Pat Garrett & Billy the Kid - Final Sahnesi Analizi, 1973",
        "year": 1973
    },
    {
        "textContent": "Toz içinde iki at izi uzanıp gidiyordu ufka doğru. Birinin izi orada duruyordu, kanlı ve keskin. Diğeri devam ediyordu, daha ağır, daha yavaş; çünkü bir şeyi arkasında bırakmıştı ve o şey hiç geri gelmeyecekti.",
        "source": "Pat Garrett & Billy the Kid - Kurgusal Prolog, 1973",
        "year": 1973
    }
]

def generate_embeddings_and_save():
    print(f"Model hazırlanıyor — {len(raw_data)} kayıt için vektörler oluşturuluyor...")
    seed_data = []

    for item in raw_data:
        try:
            embedding = model.encode(item["textContent"]).tolist()
            seed_data.append({
                "textContent": item["textContent"],
                "source": item["source"],
                "year": item["year"],
                "embedding": embedding
            })
            print(f"  ✓ {item['source']}")
        except Exception as e:
            print(f"  ✗ Hata ({item['source']}): {str(e)}")

    with open('seed.json', 'w', encoding='utf-8') as f:
        json.dump(seed_data, f, ensure_ascii=False, indent=2)

    print(f"\n✅ {len(seed_data)} kayıt → 'seed.json' oluşturuldu (384-dim vektörler).")

if __name__ == "__main__":
    generate_embeddings_and_save()