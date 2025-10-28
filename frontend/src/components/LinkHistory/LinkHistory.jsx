import { useEffect, useState } from "react";
import "./LinkHistory.scss";

export default function LinkHistory({ links: initalLinks }) {
  const [links, setLinks] = useState([]);

  useEffect(() => {
    const stored = JSON.parse(localStorage.getItem("linkHistory")) || [];
    const filtered = stored.filter(
      link => Date.now() - link.createdAt < 24 * 60 * 60 * 1000
    );

    localStorage.setItem("linkHistory", JSON.stringify(filtered));
    setLinks(filtered);
  }, [initalLinks]);

  if (!links || links.length === 0) return null;

  const handleCopy = url => {
    navigator.clipboard.writeText(url).catch(console.error);
  };

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
