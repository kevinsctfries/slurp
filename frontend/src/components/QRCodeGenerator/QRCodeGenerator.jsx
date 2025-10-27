import { useState } from "react";
import { QRCodeSVG } from "qrcode.react";
import "./QRCodeGenerator.scss";

export default function QRCodeGenerator({ url }) {
  const [copied, setCopied] = useState(false);

  if (!url) return null;

  const handleCopy = () => {
    navigator.clipboard
      .writeText(url)
      .then(() => {
        setCopied(true);
        setTimeout(() => setCopied(false), 1000);
      })
      .catch(err => console.error("Failed to copy", err));
  };

  return (
    <div className="qr-container">
      <div className="url-copy-wrapper">
        <a href={url}>{url}</a>
        <button onClick={handleCopy} className="copy-btn" aria-label="Copy URL">
          <img
            src="/copy.svg"
            alt="Copy icon"
            className={copied ? "copied" : ""}
          />
        </button>
      </div>
      <QRCodeSVG value={url} />
    </div>
  );
}
