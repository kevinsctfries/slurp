import "./ThemeToggle.scss";

export default function ThemeToggle({ theme, setTheme }) {
  const getSystemTheme = () =>
    window.matchMedia("(prefers-color-scheme: dark)").matches
      ? "dark"
      : "light";

  const toggleTheme = () => {
    setTheme(prev => {
      const newTheme = prev === "light" ? "dark" : "light";
      localStorage.setItem("theme", newTheme);
      return newTheme;
    });
  };

  const currentSystemTheme = getSystemTheme();
  const effectiveTheme = theme === "system" ? currentSystemTheme : theme;
  const iconSrc = effectiveTheme === "light" ? "/sun.svg" : "/moon.svg";

  return (
    <button
      className="theme-toggle"
      onClick={toggleTheme}
      aria-label="Toggle theme">
      <img src={iconSrc} alt="Theme toggle icon" />
    </button>
  );
}
