import { useState } from "react";
import axios from "axios";
import "./UrlShortener.scss";

function UrlShortener({ setShortUrl, setLinkHistory }) {
  const [url, setUrl] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async e => {
    e.preventDefault();

    const trimmedUrl = url.trim();

    if (!trimmedUrl) {
      setError("Please enter a URL.");
      return;
    }

    try {
      new URL(trimmedUrl);
    } catch {
      setError("Not a valid URL.");
      return;
    }

    setError("");

    const res = await axios.post(
      "http://localhost:5124/api/shorten?url=" + encodeURIComponent(url)
    );
    const shortenedUrl = res.data.shortUrl;
    setShortUrl(shortenedUrl);

    // saves links to localStorage
    const stored = JSON.parse(localStorage.getItem("linkHistory")) || [];
    const newLink = {
      original: url,
      shortened: shortenedUrl,
      createdAt: Date.now(),
    };
    const updated = [newLink, ...stored];
    localStorage.setItem("linkHistory", JSON.stringify(updated));
    setLinkHistory(updated);
  };

  const handleChange = e => {
    setUrl(e.target.value);
    setError("");
  };

  return (
    <div className="app-container">
      <h2>Paste URL:</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={url}
          onChange={handleChange}
          placeholder="Enter your URL"
        />
        <button type="submit">Shorten</button>
      </form>
      {error && <span className="error-message">{error}</span>}
    </div>
  );
}

export default UrlShortener;
