import { useState, useEffect } from "react";
import UrlShortener from "./components/UrlShortener/UrlShortener";
import QRCodeGenerator from "./components/QRCodeGenerator/QRCodeGenerator";
import "./App.scss";

function App() {
  const [shortUrl, setShortUrl] = useState("");

  const getSystemTheme = () =>
    window.matchMedia("(prefers-color-scheme: dark)").matches
      ? "dark"
      : "light";

  const [theme, setTheme] = useState(() => {
    return localStorage.getItem("theme") || "system";
  });

  useEffect(() => {
    if (theme === "system") {
      document.documentElement.removeAttribute("data-theme");
    } else {
      document.documentElement.setAttribute("data-theme", theme);
    }

    localStorage.setItem("theme", theme);

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
      if (prev === "system") {
        const newTheme = getSystemTheme() === "dark" ? "light" : "dark";
        localStorage.setItem("theme", newTheme);
        return newTheme;
      }
      const newTheme = prev === "light" ? "dark" : "light";
      localStorage.setItem("theme", newTheme);
      return newTheme;
    });
  };

  const currentSystemTheme = getSystemTheme();
  const effectiveTheme = theme === "system" ? currentSystemTheme : theme;

  const iconSrc = effectiveTheme === "light" ? "/sun.svg" : "/moon.svg";

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
