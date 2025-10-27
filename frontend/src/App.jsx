import { useState, useEffect } from "react";
import UrlShortener from "./components/UrlShortener/UrlShortener";
import QRCodeGenerator from "./components/QRCodeGenerator/QRCodeGenerator";
import ThemeToggle from "./components/ThemeToggle/ThemeToggle";
import "./App.scss";
import LinkHistory from "./components/LinkHistory/LinkHistory";

function App() {
  const [shortUrl, setShortUrl] = useState("");
  const [theme, setTheme] = useState(
    () => localStorage.getItem("theme") || "system"
  );

  useEffect(() => {
    if (theme === "system") {
      document.documentElement.removeAttribute("data-theme");
    } else {
      document.documentElement.setAttribute("data-theme", theme);
    }
    localStorage.setItem("theme", theme);
  }, [theme]);

  return (
    <div className="app-wrapper">
      <ThemeToggle theme={theme} setTheme={setTheme} />
      <h1 className="app-header">SLURP</h1>
      <h2 className="app-subheader">SLURP Links URLs Rapidly and Precisely</h2>
      <UrlShortener setShortUrl={setShortUrl} />
      {shortUrl && <QRCodeGenerator url={shortUrl} />}
      <LinkHistory />
    </div>
  );
}

export default App;
