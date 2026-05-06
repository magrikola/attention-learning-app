# Attention Learning App

Çocuklarda dikkat becerilerini desteklemek ve oyun içi dikkat performansını takip etmek için geliştirilen Unity tabanlı mobil uygulama prototipi.

Uygulamada dikkat mini oyunları, focused / unfocused durum takibi, skor sistemi ve ebeveyn rapor paneli bulunmaktadır.

---

## Projeyi Açma

### 1. Repository’yi indir

GitHub Desktop ile:

```text
File > Clone Repository > attention-learning-app

veya terminal ile:

git clone https://github.com/magrikola/attention-learning-app.git
2. Unity Hub ile aç

Unity Hub üzerinden:

Add > Add project from disk

Clone edilen proje klasörünü seç.

Seçilen klasörün içinde şu klasörler bulunmalıdır:

Assets/
Packages/
ProjectSettings/
3. Unity versiyonunu kontrol et

Projeyi açmadan önce Unity versiyonunu kontrol et:

ProjectSettings/ProjectVersion.txt

Mümkünse projeyi bu dosyada yazan Unity versiyonu ile aç. (2022.3.62f1)

İlk açılışta Unity Library klasörünü yeniden oluşturacağı için import işlemi birkaç dakika sürebilir.

Gerekli Paketler

Projede temel olarak Unity UI ve TextMeshPro kullanılmaktadır.

AR Foundation çalışmaları için Package Manager üzerinden aşağıdaki paketler kontrol edilmelidir:

AR Foundation
XR Plugin Management
ARCore XR Plugin

Android AR testi için:

Edit > Project Settings > XR Plug-in Management > Android > ARCore

ARCore aktif edilmelidir.

Git Çalışma Düzeni

Lütfen doğrudan main branch üzerinde çalışmayın.

Yeni özellikler için ayrı branch açılması önerilir:

ar-foundation
model-training
ui-polish

Örnek:

git checkout -b ar-foundation

Değişiklik sonrası:

git add .
git commit -m "Add AR Foundation setup"
git push origin ar-foundation
Büyük Dosyalar

Aşağıdaki klasör ve dosyalar GitHub’a eklenmemelidir:

Library/
Temp/
Obj/
Logs/
Build/
Builds/
*.apk
*.aab
*.pt
*.onnx
*.tflite
*.h5

Dataset, eğitilmiş model dosyaları ve build çıktıları Google Drive gibi harici bir depolama alanı üzerinden paylaşılmalıdır.

Ana Özellikler
Mobil dikkat mini oyunları
Focused / unfocused durum takibi
Skor ve süre sistemi
Günlük / haftalık ebeveyn raporu
Attention Quality ve Advanced Attention metrikleri
Demo amaçlı focus toggle sistemi
Ana Sahneler
MainMenu
GameSelection
Game1
Game2
Game3
Game4
ParentPanel

Sahneler Build Settings içinde görünmüyorsa:

File > Build Settings > Add Open Scenes
Not

Bu uygulamadaki dikkat skorları klinik tanı amacı taşımaz. Skorlar yalnızca oyun içi dikkat performansını özetlemek için kullanılmaktadır.
