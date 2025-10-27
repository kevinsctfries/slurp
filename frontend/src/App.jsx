import { useState, useEffect } from "react";
import UrlShortener from "./components/UrlShortener/UrlShortener";
import QRCodeGenerator from "./components/QRCodeGenerator/QRCodeGenerator";
import "./App.scss";

function App() {
  const [shortUrl, setShortUrl] = useState("");
  const [theme, setTheme] = useState("system");

  const getSystemTheme = () =>
    window.matchMedia("(prefers-color-scheme: dark)").matches
      ? "dark"
      : "light";

  useEffect(() => {
    if (theme === "system") {
      document.documentElement.removeAttribute("data-theme");
    } else {
      document.documentElement.setAttribute("data-theme", theme);
    }

    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    const handleSystemThemeChange = () => {
      if (theme === "system") {
        document.documentElement.removeAttribute("data-theme");
      }
    };
    mediaQuery.addEventListener("change", handleSystemThemeChange);
    return () =>
      mediaQuery.removeEventListener("change", handleSystemThemeChange);
  }, [theme]);

  const toggleTheme = () => {
    setTheme(prev => {
      if (prev === "system")
        return getSystemTheme() === "dark" ? "light" : "dark";
      return prev === "light" ? "dark" : "light";
    });
  };

  const iconSrc =
    theme === "light" || (theme === "system" && getSystemTheme() === "light")
      ? "/sun.svg"
      : "/moon.svg";

  return (
    <div className="app-wrapper">
      <button
        className="theme-toggle"
        onClick={toggleTheme}
        aria-label="Toggle theme">
        <img src={iconSrc} alt="Theme toggle icon" />
      </button>
      <h1 className="app-header">SLURP</h1>
      <h2 className="app-subheader">SLURP Links URLs Rapidly and Precisely</h2>
      <UrlShortener setShortUrl={setShortUrl} />
      {shortUrl && <QRCodeGenerator url={shortUrl} />}
    </div>
  );
}

export default App;
