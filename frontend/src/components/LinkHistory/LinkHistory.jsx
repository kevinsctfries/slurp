import { useEffect, useState } from "react";
import "./LinkHistory.scss";

export default function LinkHistory() {
  const [links, setLinks] = useState([]);

  useEffect(() => {
    const stored = JSON.parse(localStorage.getItem("linkHistory")) || [];
    setLinks(stored);
  }, []);

  const handleCopy = url => {
    navigator.clipboard.writeText(url).catch(console.error);
  };

  if (links.length === 0) return null;

  return (
    <div className="link-history">
      <h3>Link History</h3>
      <ul>
        {links.map((link, idx) => (
          <li key={idx}>
            <span className="original-url" title={link.original}>
              {link.original}
            </span>
            <a href={link.shortened} target="_blank" rel="noopener noreferrer">
              {link.shortened}
            </a>
            <button
              onClick={() => handleCopy(link.shortened)}
              className="copy-btn"
              aria-label="Copy URL">
              <img src="/copy.svg" alt="Copy icon" />
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
