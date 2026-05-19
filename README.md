# StringImagery

A Unity Editor addon that converts any text into a 512×512 white PNG image with a transparent background, perfectly centered, and saves it directly to `Assets/CustomNamePlate/Names`.

## ✨ Features

- **Instant text‑to‑texture** – type any string, click a button.
- **Fixed 512×512 resolution** – ideal for UI elements, nameplates, or decals.
- **White text, transparent background** – ready for overlaying on any material.
- **Custom font & size** – use any TrueType font; adjustable font size (8–256).
- **Automatic saving** – no file dialog; saves directly to `Assets/CustomNamePlate/Names`.
- **Smart naming** – filename derived from your text (spaces → underscores).
- **Duplicate handling** – automatically appends `_1`, `_2`, etc. if file exists.
- **Sprite import** – the resulting PNG is automatically imported as a Sprite.

## 🚀 Usage

1. Open the window: **Window → StringImagery**.
2. Enter your text in the `Text` field.
3. (Optional) Drag a font into the `Font` slot – defaults to Arial.
4. Adjust `Font Size` as needed.
5. Click **Generate Image**.
6. The PNG appears in `Assets/CustomNamePlate/Names/YourText.png` (or with a number if duplicate).

## 🧪 Example

| Input Text | Output File | Use Case |
|------------|-------------|-----------|
| `PlayerOne` | `PlayerOne.png` | VRChat nameplate text |
| `Health Bar` | `Health_Bar.png` | UI status label |
| `Welcome!` | `Welcome_.png` | Splash screen text |

## 🛠 Requirements

- Unity 2019.4 or higher (any edition).
- No external packages needed.

## 💡 Tips

- For crisp text, use a font size of at least 64–128.
- The final image is 512×512 – make sure your UI or shader uses that exact size for best results.
- Combine with **IconImagery** (sister tool) to generate circular icons for nameplates.

## 📄 License

MIT – free to use, modify, and distribute.
