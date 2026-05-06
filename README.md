````md
# Attention Learning App

Bu proje, çocuklarda dikkat becerilerini geliştirmeye ve oyun içi dikkat performansını takip etmeye yönelik Unity tabanlı bir mobil uygulama prototipidir.

Projede dikkat mini oyunları, focused / unfocused durum takibi, skor sistemi ve ebeveyn için günlük / haftalık rapor paneli bulunmaktadır.

---

## Proje Yapısı

Bu repository içinde Unity projesi için gerekli ana klasörler bulunmaktadır:

```text
Assets/
Packages/
ProjectSettings/
````

Aşağıdaki Unity tarafından otomatik oluşturulan klasörler repository içine eklenmemiştir:

```text
Library/
Temp/
Obj/
Logs/
Build/
Builds/
```

Bu klasörler proje Unity ile açıldığında otomatik olarak yeniden oluşturulur.

---

## Projeyi Açma Adımları

### 1. Repository’yi Bilgisayara İndir

Projeyi GitHub Desktop veya Git kullanarak bilgisayarına indir.

GitHub Desktop ile:

```text
File > Clone Repository > attention-learning-app
```

Git ile:

```bash
git clone https://github.com/magrikola/attention-learning-app.git
```

---

### 2. Unity Hub ile Aç

1. Unity Hub’ı aç.
2. **Add** butonuna bas.
3. **Add project from disk** seçeneğini seç.
4. Clone ettiğin proje klasörünü seç.

Seçtiğin klasörün içinde şu klasörler olmalı:

```text
Assets/
Packages/
ProjectSettings/
```

Bu klasörler görünüyorsa doğru klasörü seçmişsin demektir.

---

### 3. Unity Versiyonunu Kontrol Et

Projeyi açmadan önce Unity versiyonunu şu dosyadan kontrol et:

```text
ProjectSettings/ProjectVersion.txt
```

Mümkünse projeyi bu dosyada yazan Unity versiyonu ile aç. (2022.3.62f1)

Unity projeyi farklı bir versiyona yükseltmeyi önerirse, ekipçe karar vermeden yükseltme yapma.

---

### 4. İlk Açılış Uzun Sürebilir

Projeyi ilk kez açarken Unity `Library` klasörünü yeniden oluşturur ve assetleri import eder.

Bu işlem birkaç dakika sürebilir. Bu normaldir.

---

## Gerekli Paketler

Projede temel olarak Unity UI ve TextMeshPro kullanılmaktadır.

AR Foundation çalışmaları için aşağıdaki paketlerin yüklü olması gerekir:

```text
AR Foundation
XR Plugin Management
ARCore XR Plugin
```

Paketleri kontrol etmek veya yüklemek için:

```text
Window > Package Manager
```

Android AR testi için:

```text
Edit > Project Settings > XR Plug-in Management
Android sekmesi > ARCore aktif edilmeli
```

iOS AR desteği daha sonra gerekli olursa:

```text
ARKit XR Plugin
```

Not: iOS AR testi için macOS ve Xcode gerekir.

---

## Çalışma Düzeni

Lütfen doğrudan `main` branch üzerinde çalışmak yerine yeni bir branch oluştur.

Önerilen branch isimleri:

```text
ar-foundation
model-training
ui-polish
```

Örnek branch oluşturma:

```bash
git checkout -b ar-foundation
```

Değişiklik yaptıktan sonra:

```bash
git add .
git commit -m "Add AR Foundation setup"
git push origin ar-foundation
```

Sonrasında GitHub üzerinden Pull Request açılabilir.

---

## Büyük Dosyalar Hakkında

Aşağıdaki dosya ve klasörler repository’ye eklenmemelidir:

```text
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
```

Büyük datasetler, eğitilmiş model dosyaları ve build dosyaları GitHub’a yüklenmemelidir.

Bu dosyalar gerekiyorsa Google Drive veya başka bir harici depolama alanı üzerinden paylaşılmalıdır.

---

## Projenin Ana Özellikleri

* Mobil dikkat mini oyunları
* Focused / unfocused durum takibi
* Skor ve süre takibi
* Ebeveyn paneli
* Günlük ve haftalık rapor sistemi
* Attention Quality ve Advanced Attention metrikleri
* Demo amaçlı focus toggle sistemi

---

## Ana Sahneler

Projede kullanılan temel sahneler şunlardır:

```text
MainMenu
GameSelection
Game1
Game2
Game3
Game4
ParentPanel
```

Eğer sahneler Build Settings içinde görünmüyorsa:

```text
File > Build Settings > Add Open Scenes
```

adımı ile açık sahne Build Settings’e eklenebilir.

---

## Sorun Çıkarsa Kontrol Edilecekler

Proje açılmazsa veya hata verirse önce şunları kontrol et:

1. Doğru Unity versiyonu kullanılıyor mu?
2. Proje klasöründe `Assets`, `Packages`, `ProjectSettings` klasörleri var mı?
3. Unity asset import işlemini tamamen bitirdi mi?
4. Package Manager’da eksik paket var mı?
5. Console’da hata mesajı var mı?
6. AR için XR Plugin Management ayarları yapılmış mı?

---

## Not

Bu uygulamadaki dikkat skorları klinik tanı amacı taşımaz.

Skorlar yalnızca oyun içi dikkat performansını özetlemek için kullanılmaktadır.

```
```
