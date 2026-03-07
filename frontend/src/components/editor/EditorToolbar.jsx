// This component renders the toolbar for the TipTap editor.
// It contains formatting tools for the editor as well

import {
    FaBold, FaItalic, FaUnderline, FaListUl, FaListOl, FaUndo, FaRedo,
    FaAlignLeft, FaAlignCenter, FaAlignRight, FaHighlighter
} from "react-icons/fa";

import {MdChecklist} from "react-icons/md";

// Available colors for text formatting
const colors = [
    {name: "Black", value: "#000000"},
    {name: "Dark Gray", value: "#1c1c1c"},
    {name: "Gray", value: "#808080"},
    {name: "Light Gray", value: "#d3d3d3"},
    {name: "White", value: "#ffffff"},
    {name: "Red", value: "#ff0000"},
    {name: "Orange", value: "#ffa500"},
    {name: "Yellow", value: "#ffff00"},
    {name: "Blue", value: "#0000ff"},
    {name: "Green", value: "#008000"},
    {name: "Purple", value: "#800080"},
    {name: "Pink", value: "#ffc0cb"},
];

// Supported font sizes
const fontSizes = [8, 10, 12, 14, 16, 18, 20, 24, 32, 48];

export default function EditorToolbar({editor}) {

    // If editor has not loaded yet, do not render toolbar
    if (!editor) {
        return null;
    }

    return (
        <div className="editor-toolbar">

            {/* Undo / Redo actions */}
            <div className="toolbar-group">

                <button
                    onClick={() => editor.chain().focus().undo().run()}
                ><FaUndo/>
                </button>

                <button
                    onClick={() => editor.chain().focus().redo().run()}
                ><FaRedo/>
                </button>

            </div>

            {/* Font family selector */}
            <div className="toolbar-group">

                <select
                    onChange={(e) =>
                        editor.chain().focus().setFontFamily(e.target.value).run()
                    }
                >
                    <option value="Inter">Inter</option>
                    <option value="Arial">Arial</option>
                    <option value="Times New Roman">Times</option>
                    <option value="Courier New">Courier</option>
                </select>

            </div>

            {/* Font size selector */}
            <div className="toolbar-group">

                <select
                    onChange={(e) =>
                        editor.chain().focus().setMark("textStyle", {
                            fontSize: `${e.target.value}px`
                        }).run()
                    }
                    defaultValue="14"
                >

                    {fontSizes.map(size => {

                        return (
                            <option key={size} value={size}>
                                {size}px
                            </option>
                        )
                    })}
                </select>

            </div>

            {/* Text formatting buttons */}
            <div className="toolbar-group">

                <button
                    onClick={() => editor.chain().focus().toggleBold().run()}
                    className={editor.isActive("bold") ? "active" : ""}
                ><FaBold/>
                </button>

                <button
                    onClick={() => editor.chain().focus().toggleItalic().run()}
                    className={editor.isActive("italic") ? "active" : ""}
                ><FaItalic/>
                </button>

                <button
                    onClick={() => editor.chain().focus().toggleUnderline().run()}
                    className={editor.isActive("underline") ? "active" : ""}
                ><FaUnderline/>
                </button>

            </div>

            {/* Color selection + highlight */}
            <div className="toolbar-group">

                <select
                    onChange={(e) =>
                        editor.chain().focus().setColor(e.target.value).run()
                    }
                    defaultValue="#000000"
                >

                    {colors.map(c => {

                        // Determine text color for visibility
                        let textColor;

                        if (c.value === "#ffffff") {
                            textColor = "#000";
                        } else {
                            textColor = "#fff";
                        }

                        return (
                            <option
                                key={c.value}
                                value={c.value}
                                style={{
                                    backgroundColor: c.value,
                                    color: textColor
                                }}
                            >{c.name}
                            </option>
                        )
                    })}

                </select>

                <button
                    onClick={() =>
                        editor.chain().focus().toggleHighlight().run()
                    }
                ><FaHighlighter/>
                </button>

            </div>

            {/* Lists */}
            <div className="toolbar-group">

                <button
                    onClick={() => editor.chain().focus().toggleBulletList().run()}
                    className={editor.isActive("bulletList") ? "active" : ""}
                ><FaListUl/>
                </button>

                <button
                    onClick={() => editor.chain().focus().toggleOrderedList().run()}
                    className={editor.isActive("orderedList") ? "active" : ""}
                ><FaListOl/>
                </button>

                <button
                    onClick={() => editor.chain().focus().toggleTaskList().run()}
                    className={editor.isActive("taskList") ? "active" : ""}
                ><MdChecklist/>
                </button>

            </div>

            {/* Text alignment */}
            <div className="toolbar-group">

                <button
                    onClick={() => editor.chain().focus().setTextAlign("left").run()}
                ><FaAlignLeft/>
                </button>

                <button
                    onClick={() => editor.chain().focus().setTextAlign("center").run()}
                ><FaAlignCenter/>
                </button>

                <button
                    onClick={() => editor.chain().focus().setTextAlign("right").run()}
                ><FaAlignRight/>
                </button>

            </div>

        </div>
    );
}