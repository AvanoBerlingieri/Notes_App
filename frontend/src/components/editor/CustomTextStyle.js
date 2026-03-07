import {TextStyle} from "@tiptap/extension-text-style";

export const CustomTextStyle = TextStyle.extend({
    addGlobalAttributes() {
        return [
            {
                types: ["paragraph", "heading", "textStyle", "listItem"],
                attributes: {
                    fontSize: {
                        default: null,
                        parseHTML: el => el.style.fontSize || null,
                        renderHTML: attrs => attrs.fontSize ? {style: `font-size: ${attrs.fontSize}`} : {},
                    },
                },
            },
        ];
    },
});