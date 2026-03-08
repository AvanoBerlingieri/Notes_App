import "./Modal.css"

export default function Modal({
                                  show, title,
                                  children, onCancel,
                                  onConfirm, confirmText = "Save",
                                  confirmClass = "create-btn"
                              }) {

    if (!show) return null;

    return (
        <div className="modal-overlay" onClick={onCancel}>
            <div className="modal" onClick={(e) => e.stopPropagation()}>

                {title && <h2>{title}</h2>}

                {children}

                <div className="modal-buttons">

                    <button className="cancel-btn"
                            onClick={onCancel}
                    >Cancel
                    </button>

                    {onConfirm && (
                        <button className={confirmClass} // delete-btn, create-btn
                                onClick={onConfirm}
                        >{confirmText}
                        </button>
                    )}

                </div>
            </div>
        </div>
    );
}