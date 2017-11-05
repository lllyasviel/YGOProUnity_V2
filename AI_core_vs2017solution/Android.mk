LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)
LOCAL_MODULE := ocgcore

NDK_PROJECT_PATH := $(LOCAL_PATH)
ifndef NDEBUG
LOCAL_CFLAGS += -g -D_DEBUG
else
LOCAL_CFLAGS += -fexpensive-optimizations -O3 
endif

ifeq ($(TARGET_ARCH_ABI),x86)
LOCAL_CFLAGS += -fno-stack-protector
endif

#ifeq ($(TARGET_ARCH_ABI), armeabi-v7a)
#LOCAL_CFLAGS += -mno-unaligned-access
#endif
LOCAL_C_INCLUDES := $(LOCAL_PATH)/ocgcore

LOCAL_SRC_FILES := ocgcore/lapi.c \
				   ocgcore/lauxlib.c \
				   ocgcore/lbaselib.c \
				   ocgcore/lbitlib.c \
				   ocgcore/lcode.c \
				   ocgcore/lcorolib.c \
				   ocgcore/lctype.c \
				   ocgcore/ldblib.c \
				   ocgcore/ldebug.c \
				   ocgcore/ldo.c \
				   ocgcore/ldump.c \
				   ocgcore/lfunc.c \
				   ocgcore/lgc.c \
				   ocgcore/linit.c \
				   ocgcore/liolib.c \
				   ocgcore/llex.c \
				   ocgcore/lmathlib.c \
				   ocgcore/lmem.c \
				   ocgcore/loadlib.c \
				   ocgcore/lobject.c \
				   ocgcore/lopcodes.c \
				   ocgcore/loslib.c \
				   ocgcore/lparser.c \
				   ocgcore/lstate.c \
				   ocgcore/lstring.c \
				   ocgcore/lstrlib.c \
				   ocgcore/ltable.c \
				   ocgcore/ltablib.c \
				   ocgcore/ltm.c \
				   ocgcore/lundump.c \
				   ocgcore/lvm.c \
				   ocgcore/lzio.c \
				   ocgcore/card.cpp \
				   ocgcore/duel.cpp \
				   ocgcore/effect.cpp \
				   ocgcore/field.cpp \
				   ocgcore/group.cpp \
				   ocgcore/interpreter.cpp \
				   ocgcore/libcard.cpp \
				   ocgcore/libdebug.cpp \
				   ocgcore/libduel.cpp \
				   ocgcore/libeffect.cpp \
				   ocgcore/libgroup.cpp \
				   ocgcore/mem.cpp \
				   ocgcore/ocgapi.cpp \
				   ocgcore/operations.cpp \
				   ocgcore/playerop.cpp \
				   ocgcore/processor.cpp \
				   ocgcore/scriptlib.cpp \

LOCAL_LDLIBS := -llog
include $(BUILD_SHARED_LIBRARY)

